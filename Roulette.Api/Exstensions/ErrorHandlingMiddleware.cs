using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Roulette.Entity;
using Sentry;
using System;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;

namespace Roulette.Api.Exstensions
{
    [Description("Adds custom status codes and messages to errors")]
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context, IConfiguration configuration)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, configuration, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext httpContext, IConfiguration configuration, Exception ex)
        {
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected

            if (ex is ArgumentNullException) code = HttpStatusCode.BadRequest;

            var result = JsonConvert.SerializeObject(new { error = ex.Message });
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)code;

            //logger
            if (code == HttpStatusCode.InternalServerError)
            {
                ex.Data.Add(nameof(User), !string.IsNullOrEmpty(httpContext.Request.Query[nameof(Roulette.Entity.User.UserName)].ToString()) ? httpContext.Request.Query[nameof(Roulette.Entity.User.UserName)].ToString() : "Unknown");
                using (SentrySdk.Init(configuration["Sentry:Dsn"]))
                    SentrySdk.CaptureException(ex);
            }
            return httpContext.Response.WriteAsync(result);
        }
    }
}
