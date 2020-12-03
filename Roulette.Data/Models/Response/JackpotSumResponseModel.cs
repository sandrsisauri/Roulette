using System;
using System.Collections.Generic;
using System.Text;

namespace Roulette.Data.Models.Response
{
    public class JackpotSumResponseModel
    {
        public decimal JackpotAmount { get; init; }
        public DateTime LastModifietAt { get; init; }
    }
}
