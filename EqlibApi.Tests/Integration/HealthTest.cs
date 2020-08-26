using EqlibApi.Tests.Integration.Utils;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace EqlibApi.Tests.Integration
{
    class HealthTest
    {
        private CustomWebApplicationFactory<EqlibApi.Startup> _factory;
        private string _apiUrl;
        [OneTimeSetUp]
        public void Setup()
        {
            _factory = new CustomWebApplicationFactory<Startup>();
            _apiUrl = Environment.GetEnvironmentVariable("API_URL");
        }

        [Test]
        public async Task Health_Ping()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync(_apiUrl + "/api/v1/health");
            response.EnsureSuccessStatusCode();
            Assert.AreEqual("pong", await response.Content.ReadAsStringAsync());
        }
    }
}
