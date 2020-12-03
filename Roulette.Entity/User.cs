using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Roulette.Entity
{
    public class User : IdentityUser
    {
        public User()
        {
            Balance = 100; //lets say, user have 100$ at start;
        }
        public decimal Balance { get; set; }
    }
}
