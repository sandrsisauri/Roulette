using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Roulette.Integration.Test.LocalHelper
{
    [Trait("User", "Integration")]
    [Collection("ApiIntegrationTests")]
    public abstract class IntegrationTestFixture
            : IClassFixture<ApiIntegrationTestsFixture>
    {
        protected HttpClient Client { get; }
        protected IntegrationTestFixture(ApiIntegrationTestsFixture fixture)
        {
            Client = fixture.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions());
        }
        protected async Task<Response> GetToken(string userName = "Admin", string password = "h*{V?Nw,7?y`A*x8")
        {
            var response = await Client.PostAsJsonAsync("api/v1/user/requesttoken", new { userName, password });

            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var obj = JsonConvert.DeserializeObject<Response>(content);

            obj.Status = response.StatusCode;

            return obj;
        }
        public class Data { public string Token { get; init; }}
        public class Response { public Data Data { get; init; } public HttpStatusCode Status { get; set; } }
    }
}
