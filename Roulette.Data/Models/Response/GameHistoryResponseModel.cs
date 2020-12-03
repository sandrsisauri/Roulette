using System;
using System.Collections.Generic;
using System.Text;

namespace Roulette.Data.Models.Response
{
    public class GameHistoryResponseModel
    {
        public int Id { get; set; }
        public int BetAmount { get; set; }
        public int WonAmount { get; set; }
        public DateTime SpinDate { get; set; }
    }
}
