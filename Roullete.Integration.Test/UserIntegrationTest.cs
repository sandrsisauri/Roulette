using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Roulette.Integration.Test.LocalHelper;

namespace Roulette.Integration.Test
{
    public class UserIntegrationTest : IntegrationTestFixture
    {
        public UserIntegrationTest(ApiIntegrationTestsFixture fixture)
            : base(fixture)
        { }

        [Fact]
        public async Task Get_DefaultUser_Token_ThenOK_Test()
        {
            var response = await GetToken();

            Assert.Equal(HttpStatusCode.OK, response.Status);
        }

        [Fact]
        public async Task Get_DefaultUser_ByName_Should_Return200()
        {
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", (await GetToken()).Data.Token);

            var response = await Client.GetAsync($"api/v1/user/{Const.DefaultUser}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Get_UserByName_With_InvalidName_Should_Return404()
        {
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", (await GetToken()).Data.Token);

            var response = await Client.GetAsync($"api/v1/user/findbyname?username={Guid.NewGuid()}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Register_WithoutInput_Should_Return400()
        {
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", (await GetToken()).Data.Token);

            var response = await Client.PostAsJsonAsync("api/v1/user", JsonConvert.SerializeObject(default));

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
