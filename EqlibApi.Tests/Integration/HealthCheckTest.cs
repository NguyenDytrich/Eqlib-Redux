using Eqlib;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EqlibApi.Tests.Integration
{
    class HealthCheckTest
    {
        private TestServer _testServer;
        private HttpClient _testClient;

        [OneTimeSetUp]
        public void Setup()
        {
            var builder = new WebHostBuilder()
                .UseEnvironment(Environments.Development)
                .UseStartup<Startup>();

            _testServer = new TestServer(builder);
            _testClient = _testServer.CreateClient();
        }

        [Test]
        public async Task HealthController_Pong()
        {
            var response = await _testClient.GetAsync("/api/v1/health");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            Assert.AreEqual("pong", responseString);
        }
    }
}
