using System;
using System.Collections.Generic;
using System.Text;

namespace Roulette.Data.Models.Request
{
    public class CreateJackpotRequestModel
    {
        public decimal Amount { get; set; }
        public int BetId { get; set; }
    }
}
