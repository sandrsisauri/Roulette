using System;

namespace Roulette.Entity
{
    public class GameHistory
    {
        public int Id { get; set; }
        public int BetAmount { get; set; }
        public int WonAmount { get; set; }
        public DateTime SpinDate { get; set; }
    }
}
