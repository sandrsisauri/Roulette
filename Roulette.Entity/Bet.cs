using System;

namespace Roulette.Entity
{
    public class Bet : BaseEntity<int>
    {
        public string BetString { get; set; }
        public long BetAmount { get; set; }
        public Guid UserId { get; set; }
    }
}
