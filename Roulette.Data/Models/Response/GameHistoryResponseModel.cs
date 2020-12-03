using System;
using System.Collections.Generic;
using System.Text;

namespace Roulette.Data.Models.Response
{
    public class GameHistoryResponseModel
    {
        public int Id { get; init; }
        public int BetAmount { get; init; }
        public int WonAmount { get; init; }
        public DateTime SpinDate { get; init; }
    }
}
