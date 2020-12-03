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

namespace Roulette.Repository
{
    public class RouletteRepository : IRouletteRepository
    {
        private readonly DataContext _dataContext;

        public RouletteRepository(DataContext dataContext)
        {
            this._dataContext = dataContext;
        }

        public async Task<int> CreateBet(CreateBetRequestModel model)
        {
            return await _dataContext.Connection.InsertTimed(Mapper.Map<Bet>(model));
        }

        public async Task<int> CreateWinnings(CreateWinningsRequestModel model)
        {
            return await _dataContext.Connection.InsertTimed(Mapper.Map<Winning>(model));
        }

        public async Task<int> CreateJackPot(CreateJackpotRequestModel model)
        {
            return await _dataContext.Connection.InsertTimed(Mapper.Map<Jackpot>(model));
        }

        public async Task<IEnumerable<GameHistoryResponseModel>> GetGameHistoryByUser(Guid userId)
        {
            return await _dataContext.Connection.QueryAsync<GameHistoryResponseModel>(@"select b.Id, b.BetAmount, w.WonAmount, b.CreatedAt as SpinDate
                                                                                        from Bets b join Winnings w on w.Id = b.Id");
        }

        public async Task<JackpotSumResponseModel> GetJackpotSum()
        {
            return await _dataContext.Connection.QueryFirstOrDefaultAsync<JackpotSumResponseModel>(@"select SUM(Amount)as JackpotAmount, MAX(ModifiedAt) as LastModifietAt  from JackPots");
        }

        public async Task<UserBalanceResponseModel> GetUserBalance(Guid userId)
        {
            return await _dataContext.Connection.QuerySingleAsync<UserBalanceResponseModel>(@$"select Id as UserId, UserName, Balance from RouletteUsers where Id = '{userId}'");
        }

        public async Task AddWinToUserBalance(Guid userId, decimal amount)
        {
            await _dataContext.Connection.ExecuteAsync(@$"update RouletteUsers set Balance = (select Balance + {amount} from RouletteUsers where  Id = '{userId}' ) where Id = '{userId}'");
        }
        public async Task SubstractBetFromBalance(Guid userId, decimal amount)
        {
            await _dataContext.Connection.ExecuteAsync(@$"update RouletteUsers set Balance = (select Balance - {amount} from RouletteUsers where  Id = '{userId}' ) where Id = '{userId}'");
        }
    }
}
