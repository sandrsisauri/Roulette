using System;

namespace Roulette.Data.Models.Request
{
    public class CreateBetRequestModel
    {
        public string BetString { get; init; }
        public long BetAmount { get; init; }
        public Guid UserId { get; init; }
    }
}
