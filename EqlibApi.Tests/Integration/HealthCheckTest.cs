using Eqlib;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System;
using System.Drawing.Printing;
using System.Net.Http;
using System.Threading.Tasks;

namespace EqlibApi.Tests.Integration
{
    class HealthCheckTest
    {
        private TestServer _testServer;
        private HttpClient _testClient;
        private string _apiUrl;
        private string _endpoint = "/api/v1/health";

        [OneTimeSetUp]
        public void Setup()
        {
            var builder = new WebHostBuilder()
                .UseEnvironment(Environments.Development)
                .UseStartup<Startup>();

            _testServer = new TestServer(builder);
            _testClient = _testServer.CreateClient();
            _apiUrl = Environment.GetEnvironmentVariable("API_URL") + _endpoint;
        }

        [Test]
        public async Task HealthController_Pong()
        {
            var response = await _testClient.GetAsync(_endpoint);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            Assert.AreEqual("pong", responseString);
        }

        [Test]
        public async Task HealthController_PongWithRealClient()
        {
            var client = new HttpClient();
            var result = await client.GetAsync(_apiUrl);
            result.EnsureSuccessStatusCode();
            var resultString = await result.Content.ReadAsStringAsync();

            Assert.AreEqual("pong", resultString);
        }
    }
}
