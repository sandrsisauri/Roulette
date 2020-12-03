using System;
using System.Collections.Generic;
using System.Text;

namespace Roulette.Entity
{
    public class Winning : BaseEntity<int>
    {
        public int WinningNumber { get; set; }
        public int WonAmount { get; set; }
        public int BetId { get; set; }
        public Guid UserId { get; set; }
    }
}
