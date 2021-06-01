using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace wiremock
{
    public class BasicHttpTest : IDisposable
    {
        private readonly WireMockServer _webServiceOne;
        private readonly WireMockServer _webServiceTwo;

        public BasicHttpTest()
        {
            _webServiceOne = WireMockServer.StartWithAdminInterface(3333);
            _webServiceOne
                .Given(Request.Create().WithPath("/service-one").UsingGet())
                .RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithBody(JsonConvert.SerializeObject(new TestResponse { Msg = "Hello, service one!" }))
                );
            _webServiceTwo = WireMockServer.Start();
            _webServiceTwo
                .Given(Request.Create().WithPath("/service-two").UsingGet())
                .RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithBody(JsonConvert.SerializeObject(new TestResponse { Msg = "Hello, service two!" }))
                );
        }

        [Fact]
        public async Task ShouldGetFromServiceOne()
        {
            var response = await new HttpClient().GetAsync($"{_webServiceOne.Urls[0]}/service-one");
            var testDetails = JsonConvert.DeserializeObject<TestResponse>(await response.Content.ReadAsStringAsync());
            Assert.Equal("Hello, service one!", testDetails.Msg);
        }

        [Fact]
        public async Task ShouldGetFromServiceTwo()
        {
            var response = await new HttpClient().GetAsync($"{_webServiceTwo.Urls[0]}/service-two");
            var testDetails = JsonConvert.DeserializeObject<TestResponse>(await response.Content.ReadAsStringAsync());
            Assert.Equal("Hello, service two!", testDetails.Msg);
        }

        public void Dispose()
        {
            _webServiceOne.Dispose();
            _webServiceTwo.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}