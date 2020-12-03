using Roulette.Data.Models.Request;
using Roulette.Data.Models.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Roulette.Repository.Contract
{
    public interface IRouletteRepository
    {
        Task AddWinToUserBalance(Guid userId, decimal amount);
        Task<int> CreateBet(CreateBetRequestModel model);
        Task<int> CreateJackPot(CreateJackpotRequestModel model);
        Task<int> CreateWinnings(CreateWinningsRequestModel model);
        Task<IEnumerable<GameHistoryResponseModel>> GetGameHistoryByUser(Guid userId);
        Task<JackpotSumResponseModel> GetJackpotSum();
        Task<UserBalanceResponseModel> GetUserBalance(Guid userId);
        Task SubstractBetFromBalance(Guid userId, decimal amount);
    }
}
