using Roulette.Integration.Test.LocalHelper;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace Roulette.Integration.Test
{
    public class GeneralIntegrationTest : IntegrationTestFixture
    {
        public GeneralIntegrationTest(ApiIntegrationTestsFixture fixture)
            : base(fixture)
        { }

        [Fact]
        public async Task Each_Request_Returns_New_Token_In_Headers()
        {
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", (await GetToken()).Data.Token);
            
            var response = await Client.GetAsync($"api/v1/user/{Const.DefaultUser}");

            var newTokenFromHeader = response.Headers.GetValues("X-Response-Access-Token");

            Assert.True(newTokenFromHeader.Any());
        }

        [Fact]
        public async Task Unauthorized_User_Should_Return401()
        {
            var response = await Client.GetAsync($"api/v1/user/{Const.DefaultUser}");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
