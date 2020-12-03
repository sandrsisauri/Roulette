using ge.singular.roulette;
using Roulette.Data.Models.Request;
using Roulette.Data.Models.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Roulette.Repository.Contract
{
    public interface IRouletteRepository
    {
        Task AddWinToUserBalance(Guid userId, decimal amount, CancellationToken cancellationToken);
        Task<Response<BetResponseModel>> BetIsValidHandlerAsync(BetRequestModel request, Guid userId, CancellationToken cancellationToken);
        Task<int> CreateBet(CreateBetRequestModel model, CancellationToken cancellationToken);
        Task<int> CreateJackPot(CreateJackpotRequestModel model, CancellationToken cancellationToken);
        Task<int> CreateWinnings(CreateWinningsRequestModel model, CancellationToken cancellationToken);
        Task<Response<IEnumerable<GameHistoryResponseModel>>> GetGameHistoryByUser(Guid userId, CancellationToken cancellationToken);
        Task<Response<JackpotSumResponseModel>> GetJackpotSum(CancellationToken cancellationToken);
        Task<Response<UserBalanceResponseModel>> GetUserBalance(Guid userId, CancellationToken cancellationToken);
        Task SubstractBetFromBalance(Guid userId, decimal amount, CancellationToken cancellationToken);
    }
}
