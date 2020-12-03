using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Roulette.Data.Models.Request
{
    public class CreateBetRequestModel
    {
        public string BetString { get; set; }
        public long BetAmount { get; set; }
        public Guid UserId { get; set; }
    }
}
