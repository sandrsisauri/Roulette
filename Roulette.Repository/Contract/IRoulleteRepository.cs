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
        Task<int> CreateBet(CreateBetRequestModel model, CancellationToken cancellationToken, IDbTransaction transaction = default);
        Task<int> CreateJackPot(CreateJackpotRequestModel model, CancellationToken cancellationToken, IDbTransaction transaction = default);
        Task<int> CreateWinnings(CreateWinningsRequestModel model, CancellationToken cancellationToken, IDbTransaction transaction = default);
        Task<Response<IEnumerable<GameHistoryResponseModel>>> GetGameHistoryByUser(Guid userId, CancellationToken cancellationToken);
        Task<Response<JackpotSumResponseModel>> GetJackpotSum(CancellationToken cancellationToken);
        Task<Response<UserBalanceResponseModel>> GetUserBalance(Guid userId, CancellationToken cancellationToken, IDbTransaction transaction = default);
    }
}
