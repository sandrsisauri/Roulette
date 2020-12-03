using System;
using System.Collections.Generic;
using System.Text;

namespace Roulette.Data.Models.Response
{
    public class UserBalanceResponseModel
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public decimal Balance { get; set; }
    }
}
