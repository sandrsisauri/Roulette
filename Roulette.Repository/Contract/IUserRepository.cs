using Roulette.Entity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;

namespace Roulette.Repository.Contract
{
    public interface IUserRepository
    {
        Task Delete(object Id);
        Task<JwtSecurityToken> GenerateToken(User user);
    }
}
