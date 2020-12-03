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
using System.Net;
using ge.singular.roulette;
using System.Data;

namespace Roulette.Repository
{
    public class RouletteRepository : IRouletteRepository
    {
        private readonly DataContext _dataContext;
        private readonly IDbTransaction _transaction;

        public RouletteRepository(DataContext dataContext,
                                  IDbTransaction transaction = default)
        {
            this._dataContext = dataContext;
            this._transaction = transaction;
        }

        public async Task<int> CreateBetAsync(CreateBetRequestModel model,
                                         CancellationToken cancellationToken,
                                         IDbTransaction transaction = default)
        {
            return await _dataContext.Connection.InsertTimedAsync(Mapper.Map<Bet>(model),
                                                                  cancellationToken,
                                                                  transaction);
        }

        public async Task<int> CreateWinningsAsync(CreateWinningsRequestModel model,
                                              CancellationToken cancellationToken,
                                              IDbTransaction transaction = default)
        {
            return await _dataContext.Connection.InsertTimedAsync(Mapper.Map<Winning>(model),
                                                                  cancellationToken,
                                                                  transaction);
        }

        public async Task<int> CreateJackPotAsync(CreateJackpotRequestModel model,
                                             CancellationToken cancellationToken,
                                             IDbTransaction transaction = default)
        {
            return await _dataContext.Connection.InsertTimedAsync(Mapper.Map<Jackpot>(model),
                                                                  cancellationToken,
                                                                  transaction);
        }

        public async Task<Response<IEnumerable<GameHistoryResponseModel>>> GetGameHistoryByUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            var gamehistory = await _dataContext.Connection.QueryAsync<GameHistoryResponseModel>(@"select b.Id, b.BetAmount, w.WonAmount, b.CreatedAt as SpinDate
                                                                                        from Bets b join Winnings w on w.Id = b.Id", cancellationToken);

            return new Response<IEnumerable<GameHistoryResponseModel>>()
            {
                Data = gamehistory
            };
        }

        public async Task<Response<JackpotSumResponseModel>> GetJackpotSumAsync(CancellationToken cancellationToken)
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

        public async Task<Response<UserBalanceResponseModel>> GetUserBalanceAsync(Guid userId,
                                                                             CancellationToken cancellationToken,
                                                                             IDbTransaction transaction = default)
        {
            var userBalance = await _dataContext.Connection.QuerySingleAsync<UserBalanceResponseModel>(@$"select Id as UserId, UserName, Balance from RouletteUsers where Id = '{userId}'",
                                                                                                       cancellationToken,
                                                                                                       transaction);

            return new Response<UserBalanceResponseModel>()
            {
                Data = userBalance
            };
        }

        private async Task AddWinToUserBalanceAsync(Guid userId,
                                               decimal amount,
                                               CancellationToken cancellationToken,
                                               IDbTransaction transaction = default)
        {
            await _dataContext.Connection.ExecuteAsync(@$"update RouletteUsers set Balance = (select Balance + {amount} from RouletteUsers where  Id = '{userId}' ) where Id = '{userId}'",
                                                       cancellationToken,
                                                       transaction);
        }

        private async Task SubstractBetFromUserBalanceAsync(Guid userId,
                                                       decimal amount,
                                                       CancellationToken cancellationToken,
                                                       IDbTransaction transaction = default)
        {
            await _dataContext.Connection.ExecuteAsync(@$"update RouletteUsers set Balance = (select Balance - {amount} from RouletteUsers where  Id = '{userId}' ) where Id = '{userId}'",
                                                       cancellationToken,
                                                       transaction);
        }

        #region BetIsValidOperations
        public async Task<Response<BetResponseModel>> BetIsValidHandlerAsync(BetRequestModel request,
                                                                             Guid userId,
                                                                             CancellationToken cancellationToken)
        {
            var BetValidResponse = CheckBets.IsValid(request.Bet);

            var response = new Response<BetResponseModel>();
            if (!BetValidResponse.getIsValid())
            {
                response.ChangeStatusCode(HttpStatusCode.BadRequest, nameof(BetRequestModel.Bet) + " is not valid");
                return response;
            }

            var betAmount = BetValidResponse.getBetAmount();
            var userBalance = await GetUserBalanceAsync(userId, cancellationToken, _transaction);
            if (betAmount > userBalance.Data.Balance)
            {
                response.ChangeStatusCode(HttpStatusCode.BadRequest, nameof(BetRequestModel.Bet) + " amount can't be more than balance");
                return response;
            }
            var winNum = new Random().Next(0, 36); //not really secure, kiddo...
            var estWin = CheckBets.EstimateWin(request.Bet, winNum);
            if (estWin > 0)
                await AddWinToUserBalanceAsync(userId, estWin, cancellationToken, _transaction);

            await SubstractBetFromUserBalanceAsync(userId, betAmount, cancellationToken, _transaction);

            cancellationToken.ThrowIfCancellationRequested();

            var betId = await CreateBetAsync(
                        new CreateBetRequestModel()
                        {
                            BetString = request.Bet,
                            BetAmount = betAmount,
                            UserId = userId
                        }, cancellationToken, 
                        _transaction);

            _ = await CreateWinningsAsync(
                        new CreateWinningsRequestModel()
                        {
                            WinningNumber = winNum,
                            WonAmount = estWin,
                            UserId = userId,
                            BetId = betId
                        }, cancellationToken,
                        _transaction);

            _ = await CreateJackPotAsync(
                       new CreateJackpotRequestModel()
                       {
                           Amount = (decimal)(betAmount * 0.01),
                           BetId = betId
                       }, cancellationToken,
                       _transaction);

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
