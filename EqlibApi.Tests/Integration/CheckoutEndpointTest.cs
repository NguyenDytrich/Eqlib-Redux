using AutoFixture;
using EqlibApi.Models;
using EqlibApi.Models.Db;
using EqlibApi.Tests.Integration.Utils;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace EqlibApi.Tests.Integration
{
    [Category("Integration")]
    [NonParallelizable]
    class CheckoutEndpointTest
    {
        private CustomWebApplicationFactory<EqlibApi.Startup> _factory;
        private ApplicationContext _context;
        private string _apiUrl;
        private string _endpoint = "/api/v1/checkouts";

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _factory = new CustomWebApplicationFactory<Startup>();
            _apiUrl = Environment.GetEnvironmentVariable("API_URL");

            var optsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
            optsBuilder.UseNpgsql(Environment.GetEnvironmentVariable("CONNECTION_STRING"));

            _context = new ApplicationContext(optsBuilder.Options);

            DbContextTestHelper.ClearEntities(_context);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            DbContextTestHelper.ClearEntities(_context);
            _context.Dispose();
            _factory.Dispose();
        }

        [SetUp]
        public void SetUp()
        {
            DbContextTestHelper.ClearEntities(_context);
        }

        [Test]
        /// <summary>
        /// Get request with an empty DB should return an empty string
        /// </summary>
        public void Get_WhenEmptyDb()
        {
            var client = _factory.CreateClient();
            
            var response = Task.Run(() => client.GetAsync(_apiUrl + _endpoint)).Result;
            response.EnsureSuccessStatusCode();
            var content = Task.Run(() => response.Content.ReadAsStringAsync()).Result;

            Assert.AreEqual("[]", content);
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
        [Ignore("Temporary ignore")]
        /// <summary>
        /// Get request should return an array of checkout entries
        /// </summary>
        public async Task Get_Valid()
        {
            var fixture = new Fixture();
            var items = fixture.CreateMany<Item>().ToList();
            var checkout = fixture.Build<Checkout>()
                .With(c => c.Items, items).Create();

            _context.Items.AddRange(items);
            _context.Checkouts.Add(checkout);

            // Setup the DB with some mock entries
            var client = _factory.CreateClient();

            var expected = JsonSerializer.Serialize(checkout);

            var response = await client.GetAsync(_apiUrl + _endpoint);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            Assert.AreEqual(expected, data);
        }
    }
}
