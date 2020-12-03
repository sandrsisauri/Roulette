using System;
using System.Collections.Generic;
using System.Text;

namespace Roulette.Data.Models.Response
{
    public class UserBalanceResponseModel
    {
        public string UserId { get; init; }
        public string UserName { get; init; }
        public decimal Balance { get; init; }
    }
}
