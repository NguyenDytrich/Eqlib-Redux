using EqlibApi.Tests.Integration.Utils;
using Microsoft.AspNetCore.Server.HttpSys;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EqlibApi.Tests.Integration
{
    class CheckoutEndpointTest
    {
        private CustomWebApplicationFactory<EqlibApi.Startup> _factory;
        private string _apiUrl;
        private string _endpoint = "/api/v1/checkouts";

        [OneTimeSetUp]
        public void SetUp()
        {
            _factory = new CustomWebApplicationFactory<Startup>();
            _apiUrl = Environment.GetEnvironmentVariable("API_URL");
        }

        [Test]
        public async Task Get_WhenEmptyDb()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync(_apiUrl + _endpoint);
            response.EnsureSuccessStatusCode();
            Assert.AreEqual("[]", await response.Content.ReadAsStringAsync());
        }
    }
}
