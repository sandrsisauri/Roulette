using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Roulette.Data.Models.Request
{
    public class CreateWinningsRequestModel
    {
        public int WinningNumber { get; init; }
        public int WonAmount { get; init; }
        public int BetId { get; init; }
        public Guid UserId { get; init; }
    }
}
