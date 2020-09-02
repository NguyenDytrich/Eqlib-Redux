using AutoFixture;
using EqlibApi.Models;
using EqlibApi.Models.Db;
using EqlibApi.Models.Enums;
using EqlibApi.Tests.Integration.Utils;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Text.Json;
using System.Threading;
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
        public void Get_NonExistant()
        {
            var client = _factory.CreateClient();
            var response = Task.Run(() => client.GetAsync(_apiUrl + _endpoint + "/400")).Result;

            var status = response.StatusCode;
            Assert.AreEqual(HttpStatusCode.NotFound, status);
        }

        [Test]
        /// <summary>
        /// Get request should return an array of checkout entries
        /// </summary>
        public void Get_Valid()
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

            var response = Task.Run(() => client.GetAsync(_apiUrl + _endpoint)).Result;
            response.EnsureSuccessStatusCode();

            var jsonString = Task.Run(() => response.Content.ReadAsStringAsync()).Result;
            Console.WriteLine(jsonString);
            var serializedJson = JsonSerializer.Deserialize<Get_ExpectedResponse>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            serializedJson.Checkouts.Should().ContainEquivalentOf(checkoutFromDb, opts =>
                opts.Excluding(c => c.Items)
                    .Excluding(c => c.ItemIds)
                    .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, 1000))
                        .WhenTypeIs<DateTime>());
        }

        [Test]
        [TestCaseSource("Valid_PostJsonBodies")]
        /// <summary>
        /// Post request with valid entries should be processed correctly.
        /// </summary>
        public void Post_Valid(Post_JsonBody jsonBody)
        {
            var fixture = new Fixture();
            var itemFixtures = fixture.Build<Item>()
                .With(i => i.Availability, EAvailability.Available)
                .CreateMany().ToDictionary(i => i.Id, i => i);

            var itemIds = new List<int>(itemFixtures.Keys);
            jsonBody.ItemIds = itemIds;
            var jsonString = JsonSerializer.Serialize(jsonBody);

            _context.Items.AddRange(itemFixtures.Values);
            _context.SaveChanges();
            var client = _factory.CreateClient();

            var response = Task.Run(() => client.PostAsync(_apiUrl + _endpoint,
                new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json"))).Result;
            response.EnsureSuccessStatusCode();

            var checkout = _context.Checkouts.FirstOrDefault();

            checkout.DueDate.Should().BeSameDateAs((DateTime)jsonBody.DueDate);
            checkout.CheckoutDate.Should().BeSameDateAs((DateTime)(jsonBody.CheckoutDate == null ? DateTime.Now : jsonBody.CheckoutDate));
            checkout.ApprovalStatus.Should().Be(jsonBody.ApprovalStatus);

            var items = _context.Items.AsNoTracking().ToList();
            foreach (Item i in items)
            {
                Console.WriteLine(JsonSerializer.Serialize(i));
                Assert.AreEqual(EAvailability.CheckedOut, i.Availability);
            }
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

            Assert.AreEqual("{\"checkouts\":[]}", content);
        }

        #region Helper classes
        /// <summary>
        /// Container class for JSON deserialization
        /// </summary>
        public class Get_ExpectedResponse
        {
            public IList<Checkout> Checkouts { get; set; }

            public Get_ExpectedResponse() { }
        }

        /// <summary>
        /// Container class for a JSON checkout POST request
        /// </summary>
        public class Post_JsonBody
        {
            public List<int> ItemIds { get; set; }
            public  DateTime? DueDate { get; set; }
            public DateTime? CheckoutDate { get; set; }
            public ECheckoutApproval? ApprovalStatus { get; set; }

            public Post_JsonBody() { }
        }

        /// <summary>
        /// Generates valid Post_JsonBody objects
        /// </summary>
        public static IEnumerable<Post_JsonBody> Valid_PostJsonBodies
        {
            get
            {
                yield return new Post_JsonBody()
                {
                    DueDate = DateTime.Now.AddDays(7),
                    ApprovalStatus = ECheckoutApproval.Approved
                };

                yield return new Post_JsonBody()
                {
                    DueDate = DateTime.Now.AddDays(5),
                    ApprovalStatus = ECheckoutApproval.Pending
                };

                yield return new Post_JsonBody()
                {
                    DueDate = DateTime.Now.AddDays(7),
                    ApprovalStatus = ECheckoutApproval.Denied
                };

                yield return new Post_JsonBody()
                {
                    CheckoutDate = new DateTime(),
                    DueDate = new DateTime().AddDays(14),
                    ApprovalStatus = ECheckoutApproval.Approved
                };

                yield return new Post_JsonBody()
                {
                    CheckoutDate = DateTime.Now.AddDays(-7),
                    DueDate = new DateTime(),
                    ApprovalStatus = ECheckoutApproval.Denied
                };
            }
        }
        #endregion
    }
}