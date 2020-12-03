using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roulette.Data.Models.Response
{
    public class CreateUserResponseModel
    {
        public Guid UserId { get; init; }
        public string Token { get; init; }
    }
}
