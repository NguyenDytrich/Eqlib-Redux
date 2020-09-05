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
using System.Text.Json;
using System.Threading.Tasks;

namespace EqlibApi.Tests.Integration
{

    abstract class CheckoutTestBase
    {
        protected CustomWebApplicationFactory<EqlibApi.Startup> _factory;
        //protected ApplicationContext _context;
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
            var _context = DbContextTestHelper.CreateAppContext();
            DbContextTestHelper.DeleteAllFromDb(_context);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            var _context = DbContextTestHelper.CreateAppContext();
            DbContextTestHelper.DeleteAllFromDb(_context);

            _context.Dispose();
            _factory.Dispose();
        }

        [TearDown]
        public void TearDown()
        {
            var context = DbContextTestHelper.CreateAppContext();
            DbContextTestHelper.DeleteAllFromDb(context);
            context.Dispose();
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
            var context = DbContextTestHelper.CreateAppContext();
            var itemFixtures = DbContextTestHelper.PopulateWithItems(context);
            var checkout = fixture.Build<Checkout>()
                .With(c => c.Items, itemFixtures.ToList()).Create();

            context.Add(checkout);
            context.SaveChanges();

            var itemIds = itemFixtures.Select(f => f.Id).ToList();
            var client = _factory.CreateClient();

            var response = Task.Run(() => client.GetAsync(_apiUrl + _endpoint)).Result;
            response.EnsureSuccessStatusCode();

            var checkoutFromDb = context.Checkouts.Find(checkout.Id);

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

        [Test]
        [TestCaseSource("Valid_PostJsonBodies")]
        /// <summary>
        /// Post request with valid entries should be processed correctly.
        /// </summary>
        public void Post_Valid(Post_JsonBody jsonBody)
        {
            var context = DbContextTestHelper.CreateAppContext();

            var itemFixtures = DbContextTestHelper.PopulateWithItems(context, EAvailability.Available);
            var itemIds = itemFixtures.Select(f => f.Id).ToList();
            var client = _factory.CreateClient();

            jsonBody.ItemIds = itemIds;
            var jsonString = JsonSerializer.Serialize(jsonBody);
            var response = Task.Run(() => client.PostAsync(_apiUrl + _endpoint,
                new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json"))).Result;
            Console.WriteLine(Task.Run(() => response.Content.ReadAsStringAsync()).Result);
            response.EnsureSuccessStatusCode();

            var checkout = context.Checkouts.FirstOrDefault();

            checkout.DueDate.Should().BeSameDateAs((DateTime)jsonBody.DueDate);

            // Checkout date should default to current date.
            checkout.CheckoutDate.Should().BeSameDateAs(
                (DateTime)(jsonBody.CheckoutDate == null ? DateTime.Now : jsonBody.CheckoutDate));

            checkout.ApprovalStatus.Should().Be(jsonBody.ApprovalStatus);

            var items = context.Items.AsNoTracking().ToList();
            foreach (Item i in items)
            {
                Assert.AreEqual(EAvailability.CheckedOut, i.Availability);
            }
        }

        [Test]
        [TestCaseSource("InvalidDueDate_PostJsonBodies")]
        public void Post_InvalidDueDate(Post_JsonBody jsonBody)
        {
            var context = DbContextTestHelper.CreateAppContext();
            var itemFixtures = DbContextTestHelper.PopulateWithItems(context, EAvailability.Available);
            var itemIds = itemFixtures.Select(f => f.Id).ToList();
            var client = _factory.CreateClient();

            jsonBody.ItemIds = itemIds;
            var jsonString = JsonSerializer.Serialize(jsonBody);
            var response = Task.Run(() => client.PostAsync(_apiUrl + _endpoint,
                new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json"))).Result;
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);

            var content = Task.Run(() => response.Content.ReadAsStringAsync()).Result;
            content.Should().Contain("Due date must be after checkout date.");
        }

        [Test]
        [TestCaseSource("Valid_PostJsonBodies")]
        public void Post_ItemNotFound(Post_JsonBody jsonBody)
        {
            var client = _factory.CreateClient();
            var fixture = new Fixture();
            jsonBody.ItemIds = fixture.CreateMany<int>().ToList();
            var jsonString = JsonSerializer.Serialize(jsonBody);

            var response = Task.Run(() => client.PostAsync(_apiUrl + _endpoint,
                new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json"))).Result;
            var content = Task.Run(() => response.Content.ReadAsStringAsync()).Result;

            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
            content.Should().Contain("not exist");
        }

        [Test]
        [TestCaseSource("Valid_PostJsonBodies")]
        public void Post_ItemNotAvailable(Post_JsonBody jsonBody)
        {
            var client = _factory.CreateClient();
            var context = DbContextTestHelper.CreateAppContext();
            var itemFixtures = DbContextTestHelper.PopulateWithItems(context, EAvailability.CheckedOut);
            var itemIds = itemFixtures.Select(i => i.Id).ToList();
            jsonBody.ItemIds = itemIds;
            var jsonString = JsonSerializer.Serialize(jsonBody);

            var response = Task.Run(() => client.PostAsync(_apiUrl + _endpoint,
                new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json"))).Result;
            var content = Task.Run(() => response.Content.ReadAsStringAsync()).Result;

            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
            content.Should().Contain("not available");
        }

        [Test]
        public void Post_InvalidReturnDate()
        {
            var context = DbContextTestHelper.CreateAppContext();
            var itemFixtures = DbContextTestHelper.PopulateWithItems(context);
            var itemIds = itemFixtures.Select(f => f.Id).ToList();
            var client = _factory.CreateClient();

            var jsonBody = new Post_JsonBody()
            {
                CheckoutDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(7),
                ReturnDate = DateTime.Now.AddDays(-1),
                ItemIds = new List<int>()
            };

            var jsonString = JsonSerializer.Serialize(jsonBody);
            var response = Task.Run(() => client.PostAsync(_apiUrl + _endpoint,
                new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json"))).Result;
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);

            var content = Task.Run(() => response.Content.ReadAsStringAsync()).Result;
            content.Should().Contain("Return date must be after checkout date.");
        }

        [Test]
        [TestCase(12)]
        /// <summary>
        /// DETETE request should remove the entry from the database and mark items as available.
        /// </summary>
		public void Delete_Valid(int checkoutId) {
            var context = DbContextTestHelper.CreateAppContext();
            var itemFixtures = DbContextTestHelper.PopulateWithItems(context, EAvailability.CheckedOut);

            var fixture = new Fixture();
            var checkout = fixture.Build<Checkout>()
                .With(c => c.Items, itemFixtures.ToList())
                .With(c => c.Id, checkoutId)
                .Create();

            context.Add(checkout);
            context.SaveChanges();

			var client = _factory.CreateClient();
			var response = Task.Run(() => client.DeleteAsync($"{_apiUrl}{_endpoint}/{checkoutId}")).Result;
			response.EnsureSuccessStatusCode();

            var dbCheckouts = context.Checkouts.AsNoTracking().Where(c => c.Id == checkoutId).ToList();
            Assert.AreEqual(0, dbCheckouts.Count());

            var items = context.Items.AsNoTracking().ToList();
            foreach (Item i in items)
            {
                Assert.AreEqual(EAvailability.Available, i.Availability);
            }
		}

        [Test]
        /// <summary>
        /// Requests to a non-existant entity should return 404
        /// </summary>
        public void Delete_NotFound()
        {
            var client = _factory.CreateClient();
            var response = Task.Run(() => client.DeleteAsync(_apiUrl + _endpoint + "/400")).Result;

            var status = response.StatusCode;
            Assert.AreEqual(HttpStatusCode.NotFound, status);
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
            public DateTime? DueDate { get; set; }
            public DateTime? CheckoutDate { get; set; }
            public DateTime? ReturnDate { get; set; }
            public ECheckoutApproval ApprovalStatus { get; set; }

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
                    CheckoutDate = DateTime.Now,
                    DueDate = DateTime.Now.AddDays(14),
                    ApprovalStatus = ECheckoutApproval.Approved
                };

                yield return new Post_JsonBody()
                {
                    CheckoutDate = DateTime.Now.AddDays(-7),
                    DueDate = DateTime.Now,
                    ApprovalStatus = ECheckoutApproval.Denied
                };

                yield return new Post_JsonBody()
                {
                    DueDate = DateTime.Now.AddDays(5),
                };
            }
        }

        public static IEnumerable<Post_JsonBody> InvalidDueDate_PostJsonBodies
        {
            get
            {
                yield return new Post_JsonBody()
                {
                    DueDate = DateTime.Now.AddDays(-1),
                    CheckoutDate = DateTime.Now.AddDays(2)
                };

                yield return new Post_JsonBody()
                {
                    DueDate = DateTime.Now.AddDays(-2)
                };
            }
        }
        #endregion
    }
}
