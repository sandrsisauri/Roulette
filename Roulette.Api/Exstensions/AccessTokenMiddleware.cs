using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Roulette.Api.Controllers.v1;
using Roulette.Repository.Contract;
using Roulette.Entity;
using Roulette.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Roulette.Api.Exstensions
{
    [Description("Adds new token for each request")]
    public class AccessTokenMiddleware
    {
        private readonly RequestDelegate _next;

        public AccessTokenMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IUserRepository userRepository, UserManager<User> userManager)
        {
            var token = await context.GetTokenAsync(Const.access_token);
            var isAuthenticated = !string.IsNullOrEmpty(token);

            if (!isAuthenticated)
            {
                await _next(context);
            }
            else
            {
                var userid = ControllerBaseEx.GetUserIdFromToken(token);
                var user = await userManager.FindByIdAsync(userid.ToString());
                var newtoken = await userRepository.GenerateToken(user);

                context.Response.OnStarting(state =>
                {
                    var httpContext = (HttpContext)state;
                    httpContext.Response.Headers.Add("X-Response-Access-Token", new[] { token });

                    return Task.CompletedTask;
                }, context);

                await _next(context);
            }
        }
    }
}
