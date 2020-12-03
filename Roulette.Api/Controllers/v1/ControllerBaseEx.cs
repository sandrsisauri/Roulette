using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Roulette.Helper;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace Roulette.Api.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    public class ControllerBaseEx : ControllerBase
    {
        internal static Guid GetUserIdFromTokenAsync(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var userid = (handler.ReadToken(token) as JwtSecurityToken).Claims.First(claim => claim.Type == Const.UserIdClaim).Value;
            return Guid.Parse(userid);
        }
    }
}
