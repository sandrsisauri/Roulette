using Roulette.Data.Models.Request;
using Roulette.Data.Models.Response;
using Roulette.Entity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Roulette.Repository.Contract
{
    public interface IUserRepository
    {
        Task<Response<CreateUserResponseModel>> CreateUser(CreateUserRequestModel input, CancellationToken cancellationToken);
        Task<JwtSecurityToken> GenerateToken(User user);
        Task<Response<TokenResponseModel>> GetToken(LoginUserRequestModel input, CancellationToken cancellationToken);
        Task<Response<UserResponseModel>> GetUserByName(string userName, CancellationToken cancellationToken);
    }
}
