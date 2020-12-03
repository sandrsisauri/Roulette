using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace Roulette.Api.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    public class ControllerBaseEx : ControllerBase {
        public static Guid GetUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var tokenS = handler.ReadToken(token) as JwtSecurityToken;
            var userid = tokenS.Claims.First(claim => claim.Type == "UserId").Value;
            return Guid.Parse(userid);
        }
    }
}
