using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Roulette.Entity;
using Sentry;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Roulette.Api.Exstensions
{
    [Description("Adds log for each request")]
    public class HttpContextMiddleware
    {
        readonly RequestDelegate _next;

        public HttpContextMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext httpContext, IConfiguration configuration)
        {
            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));

            string userName = !string.IsNullOrEmpty(httpContext.Request.Query[nameof(User.UserName)].ToString())
                ? httpContext.Request.Query[nameof(User.UserName)].ToString()
                : "Unknown";

            var ip = httpContext.Request.HttpContext.Connection.RemoteIpAddress?.ToString();

            var sw = Stopwatch.StartNew();

            sw.Stop();
            var MessageTemplate = $"HTTP {httpContext.Request.Method} {httpContext.Request.Path} responded {httpContext.Response.StatusCode} in {sw.Elapsed.TotalMilliseconds:0.0000} ms [User:{userName}], with ip {ip}";

            using (SentrySdk.Init(configuration["Sentry:Dsn"]))
                SentrySdk.CaptureMessage(MessageTemplate, Sentry.Protocol.SentryLevel.Info);

            await _next(httpContext);
        }
    }
}
