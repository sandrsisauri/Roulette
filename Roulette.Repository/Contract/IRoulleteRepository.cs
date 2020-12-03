using Roulette.Data.Models.Request;
using Roulette.Data.Models.Response;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Roulette.Repository.Contract
{
    public interface IRouletteRepository
    {
        Task<Response<BetResponseModel>> BetIsValidHandlerAsync(BetRequestModel request, Guid userId, CancellationToken cancellationToken);
        Task<int> CreateBetAsync(CreateBetRequestModel model, CancellationToken cancellationToken, IDbTransaction transaction = default);
        Task<int> CreateJackPotAsync(CreateJackpotRequestModel model, CancellationToken cancellationToken, IDbTransaction transaction = default);
        Task<int> CreateWinningsAsync(CreateWinningsRequestModel model, CancellationToken cancellationToken, IDbTransaction transaction = default);
        Task<Response<IEnumerable<GameHistoryResponseModel>>> GetGameHistoryByUserAsync(Guid userId, CancellationToken cancellationToken);
        Task<Response<JackpotSumResponseModel>> GetJackpotSumAsync(CancellationToken cancellationToken);
        Task<Response<UserBalanceResponseModel>> GetUserBalanceAsync(Guid userId, CancellationToken cancellationToken, IDbTransaction transaction = default);
    }
}
