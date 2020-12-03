using System;
using System.Collections.Generic;
using System.Text;

namespace Roulette.Entity
{
    public class Jackpot : BaseEntity<int>
    {
        public decimal Amount { get; set; }
        public int BetId { get; set; }
    }
}
