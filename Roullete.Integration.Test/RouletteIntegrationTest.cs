using Newtonsoft.Json;
using Roulette.Data.Models.Response;
using Roulette.Integration.Test;
using Roulette.Integration.Test.LocalHelper;
using Roullete.Integration.Test.LocalHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Roullete.Integration.Test
{
    public class RouletteIntegrationTest : IntegrationTestFixture
    {
        public RouletteIntegrationTest(ApiIntegrationTestsFixture fixture)
            : base(fixture)
        { }

        [Fact]
        public async Task Get_GameHistory_Should_Return_200()
        {
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", (await GetToken()).Data.Token);

            var response = await Client.GetAsync($"api/v1/roulette/gamehistory");

            var obj = JsonConvert.DeserializeObject<Response<IEnumerable<GameHistoryResponseModel>>>(await response.Content.ReadAsStringAsync());

            Assert.Null(obj.Message);
            Assert.NotNull(obj);
            Assert.Equal((int)HttpStatusCode.OK, obj.StatusCode);
        }
        [Fact]
        public async Task Get_Jackpot_Response_Null_Check()
        {
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", (await GetToken()).Data.Token);

            var response = await Client.GetAsync($"api/v1/roulette/jackpot");

            var obj = JsonConvert.DeserializeObject<Response<JackpotSumResponseModel>>(await response.Content.ReadAsStringAsync());

            Assert.Null(obj.Message);
            Assert.NotNull(obj);
            Assert.Equal((int)HttpStatusCode.OK, obj.StatusCode);
        }
    }
}
