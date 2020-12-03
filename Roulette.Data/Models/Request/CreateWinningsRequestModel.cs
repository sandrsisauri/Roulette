using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Roulette.Data.Models.Request
{
    public class CreateWinningsRequestModel
    {
        public int WinningNumber { get; set; }
        public int WonAmount { get; set; }
        public int BetId { get; set; }
        public Guid UserId { get; set; }
    }
}
