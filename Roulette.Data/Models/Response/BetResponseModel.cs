using System;
using System.Collections.Generic;
using System.Text;

namespace Roulette.Data.Models.Response
{
    public class BetResponseModel
    {
        public long BetAmount { get; init; }
        public int WinningAmount { get; init; }
    }
}
