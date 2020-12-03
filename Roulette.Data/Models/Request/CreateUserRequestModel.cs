using System;
using System.Collections.Generic;
using System.Text;

namespace Roulette.Data.Models.Request
{
    public class CreateUserRequestModel
    {
        public string UserName { get; init; }
        public string Password { get; init; }

        //confirm password property maybe?
    }
}
