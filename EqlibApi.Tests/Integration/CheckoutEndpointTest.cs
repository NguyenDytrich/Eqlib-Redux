using AutoFixture;
using EqlibApi.Models;
using EqlibApi.Models.Db;
using EqlibApi.Tests.Integration.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EqlibApi.Tests.Integration
{
    [Category("Integration")]
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
        /// <summary>
        /// Get request with an empty DB should return an empty string
        /// </summary>
        public async Task Get_WhenEmptyDb()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync(_apiUrl + _endpoint);
            response.EnsureSuccessStatusCode();

            var content = response.Content;
            Assert.AreEqual(Encoding.UTF8.GetBytes("[]"), await response.Content.ReadAsByteArrayAsync());
        }

        [Test]
        /// <summary>
        /// Get request to non-existant ID should return 404
        /// </summary>
        public async Task Get_NonExistant()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync(_apiUrl + _endpoint + "/400");

            var status = response.StatusCode;
            Assert.AreEqual(HttpStatusCode.NotFound, status);
        }

        [Test]
        /// <summary>
        /// Get request should return an array of checkout entries
        /// </summary>
        public async Task Get_Valid()
        {
            var fixture = new Fixture();

            // Setup the DB with some mock entries
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var sp = services.BuildServiceProvider();

                    using var scope = sp.CreateScope();
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<ApplicationContext>();

                    db.Database.EnsureCreated();
                    db.SaveChanges();
                });
            }).CreateClient();

            var response = await client.GetAsync(_apiUrl + _endpoint);
            response.EnsureSuccessStatusCode();

            Assert.AreEqual("", await response.Content.ReadAsStringAsync());
        }
    }
}
