using AutoFixture;
using EqlibApi.Models;
using EqlibApi.Models.Db;
using EqlibApi.Tests.Integration.Utils;
using FluentAssertions;
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

    abstract class CheckoutTestBase
    {
        protected CustomWebApplicationFactory<EqlibApi.Startup> _factory;
        protected ApplicationContext _context;
        protected string _apiUrl;
        protected string _endpoint = "/api/v1/checkouts";
    }

    [Category("Integration")]
    [NonParallelizable]
    class CheckoutEndpointTest : CheckoutTestBase
    {
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

        [TearDown]
        public void TearDown()
        {
            DbContextTestHelper.ClearEntities(_context);
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
            var items = fixture.CreateMany<Item>().ToList();
            var checkout = fixture.Build<Checkout>()
                .With(c => c.Items, items).Create();

            _context.Items.AddRange(items);
            _context.Checkouts.Add(checkout);
            _context.SaveChanges();

            var client = _factory.CreateClient();
            var checkoutFromDb = _context.Checkouts.Find(checkout.Id);

            var response = await client.GetAsync(_apiUrl + _endpoint);
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            Console.WriteLine(jsonString);
            var serializedJson = JsonSerializer.Deserialize<ExpectedResponse>(jsonString);
            serializedJson.Checkouts.Should().ContainEquivalentOf(checkoutFromDb, opts => opts.ExcludingNestedObjects());
        }
    }

    [Category("Integration")]
    class CheckoutEndpointTest_EmptyDb : CheckoutTestBase
    {
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

        [Test]
        /// <summary>
        /// Get request with an empty DB should return an empty string
        /// </summary>
        public async Task Get_WhenEmptyDb()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync(_apiUrl + _endpoint);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            Assert.AreEqual("{\"checkouts\":[]}", content);
        }
    }

    class ExpectedResponse
    {
        public IList<Checkout> Checkouts { get; set; }

        public ExpectedResponse() { }
    }
}
