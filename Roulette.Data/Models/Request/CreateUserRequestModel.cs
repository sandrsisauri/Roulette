using System;
using System.Collections.Generic;
using System.Text;

namespace Roulette.Data.Models.Request
{
    public class CreateUserRequestModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        //confirm password property maybe?
    }
}
