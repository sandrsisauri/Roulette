using AutoMapper;
using Dapper;
using Roulette.Data;
using Roulette.Data.Models.Request;
using Roulette.Data.Models.Response;
using Roulette.Repository.Contract;
using Roulette.Repository.LocalHelper;
using Roulette.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Roulette.Helper;
using System.Net;
using ge.singular.roulette;

namespace Roulette.Repository
{
    public class RouletteRepository : IRouletteRepository
    {
        private readonly DataContext _dataContext;

        public RouletteRepository(DataContext dataContext)
        {
            this._dataContext = dataContext;
        }

        public async Task<int> CreateBet(CreateBetRequestModel model, CancellationToken cancellationToken)
        {
            return await _dataContext.Connection.InsertTimedAsync(Mapper.Map<Bet>(model), cancellationToken);
        }

        public async Task<int> CreateWinnings(CreateWinningsRequestModel model, CancellationToken cancellationToken)
        {
            return await _dataContext.Connection.InsertTimedAsync(Mapper.Map<Winning>(model), cancellationToken);
        }

        public async Task<int> CreateJackPot(CreateJackpotRequestModel model, CancellationToken cancellationToken)
        {
            return await _dataContext.Connection.InsertTimedAsync(Mapper.Map<Jackpot>(model), cancellationToken);
        }

        public async Task<Response<IEnumerable<GameHistoryResponseModel>>> GetGameHistoryByUser(Guid userId, CancellationToken cancellationToken)
        {
            var gamehistory = await _dataContext.Connection.QueryAsync<GameHistoryResponseModel>(@"select b.Id, b.BetAmount, w.WonAmount, b.CreatedAt as SpinDate
                                                                                        from Bets b join Winnings w on w.Id = b.Id", cancellationToken);

            return new Response<IEnumerable<GameHistoryResponseModel>>()
            {
                Data = gamehistory
            };
        }

        public async Task<Response<JackpotSumResponseModel>> GetJackpotSum(CancellationToken cancellationToken)
        {
            var jackpot = await _dataContext.Connection.QueryFirstOrDefaultAsync<JackpotSumResponseModel>(@"select SUM(Amount)as JackpotAmount, MAX(ModifiedAt) as LastModifietAt  from JackPots", cancellationToken);

            var response = new Response<JackpotSumResponseModel>()
            {
                Data = jackpot
            };

            if (jackpot == null)
                response.ChangeStatusCode(HttpStatusCode.NotFound, nameof(jackpot));

            return response;
        }

        public async Task<Response<UserBalanceResponseModel>> GetUserBalance(Guid userId, CancellationToken cancellationToken)
        {
            var userBalance = await _dataContext.Connection.QuerySingleAsync<UserBalanceResponseModel>(@$"select Id as UserId, UserName, Balance from RouletteUsers where Id = '{userId}'", cancellationToken);

            return new Response<UserBalanceResponseModel>()
            {
                Data = userBalance
            };
        }

        public async Task AddWinToUserBalance(Guid userId, decimal amount, CancellationToken cancellationToken)
        {
            await _dataContext.Connection.ExecuteAsync(@$"update RouletteUsers set Balance = (select Balance + {amount} from RouletteUsers where  Id = '{userId}' ) where Id = '{userId}'", cancellationToken);
        }

        public async Task SubstractBetFromBalance(Guid userId, decimal amount, CancellationToken cancellationToken)
        {
            await _dataContext.Connection.ExecuteAsync(@$"update RouletteUsers set Balance = (select Balance - {amount} from RouletteUsers where  Id = '{userId}' ) where Id = '{userId}'", cancellationToken);
        }

        #region BetIsValidOperations
        public async Task<Response<BetResponseModel>> BetIsValidHandlerAsync(BetRequestModel request, Guid userId, CancellationToken cancellationToken)
        {
            var BetValidResponse = CheckBets.IsValid(request.Bet);

            var response = new Response<BetResponseModel>();
            if (!BetValidResponse.getIsValid())
            {
                response.ChangeStatusCode(HttpStatusCode.BadRequest, nameof(BetRequestModel.Bet) + " is not valid");
                return response;
            }

            var betAmount = BetValidResponse.getBetAmount();
            var userBalance = await GetUserBalance(userId, cancellationToken);
            if (betAmount > userBalance.Data.Balance)
            {
                response.ChangeStatusCode(HttpStatusCode.BadRequest, nameof(BetRequestModel.Bet) + " amount can't be more than balance");
                return response;
            }
            var winNum = new Random().Next(0, 36);
            var estWin = CheckBets.EstimateWin(request.Bet, winNum);
            if (estWin > 0)
                await AddWinToUserBalance(userId, estWin, cancellationToken);

            await SubstractBetFromBalance(userId, betAmount, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            var betId = await CreateBet(
                        new CreateBetRequestModel()
                        {
                            BetString = request.Bet,
                            BetAmount = betAmount,
                            UserId = userId
                        }, cancellationToken);

            _ = await CreateWinnings(
                        new CreateWinningsRequestModel()
                        {
                            WinningNumber = winNum,
                            WonAmount = estWin,
                            UserId = userId,
                            BetId = betId
                        }, cancellationToken);

            _ = await CreateJackPot(
                       new CreateJackpotRequestModel()
                       {
                           Amount = (decimal)(betAmount * 0.01),
                           BetId = betId
                       }, cancellationToken);

            return new Response<BetResponseModel>()
            {
                Data = new BetResponseModel()
                {
                    BetAmount = betAmount,
                    WinningAmount = estWin
                }
            };
        }
        #endregion
    }
}
