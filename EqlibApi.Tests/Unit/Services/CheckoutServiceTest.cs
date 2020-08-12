using AutoFixture;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using EqlibApi.Models;
using EqlibApi.Models.Db;
using EqlibApi.Services;
using EqlibApi.Tests.Unit.Utils;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EqlibApi.Tests.Unit.Services
{
    class CheckoutServiceTest
    {
        private CheckoutService checkoutService;
        private Mock<IApplicationContext> contextMock;

        #region Utils
        public static IEnumerable<TestCaseData> checkoutsProvider()
        {
            var fixture = new Fixture();

            yield return new TestCaseData(fixture.CreateMany<Checkout>().ToList());
            yield return new TestCaseData(fixture.CreateMany<Checkout>().ToList());
            yield return new TestCaseData(fixture.CreateMany<Checkout>().ToList());
        }
        public static IEnumerable<TestCaseData> checkoutRequestProvider()
        {
            var fixture = new Fixture();
            var mock = fixture.Build<Checkout>().Without(c => c.CheckoutStatus);
            yield return new TestCaseData(mock.Create());
        }
        #endregion

        [SetUp]
        protected void SetUp()
        {
            contextMock = new Mock<IApplicationContext>();
            checkoutService = new CheckoutService(contextMock.Object);
        }

        #region Get Tests
        [Test, TestCaseSource("checkoutsProvider")]
        public async Task GetAsync_Valid(List<Checkout> checkouts)
        {
            contextMock.Setup(c => c.Checkouts).Returns(DbSetProvider.CreateSet(checkouts));

            var result = await checkoutService.GetAsync();
            result.Should().BeEquivalentTo(checkouts);
        }

        [Test, TestCaseSource("checkoutsProvider")]
        public async Task GetAsync_ValidById(List<Checkout> checkouts)
        {
            contextMock.Setup(c => c.Checkouts).Returns(DbSetProvider.CreateSet(checkouts));
            var targetId = checkouts.FirstOrDefault().Id;

            var result = await checkoutService.GetAsync(c => c.Id == targetId);
            result.Should().BeEquivalentTo(checkouts.FirstOrDefault());
        }
        #endregion

        #region Delete Tests
        [Test, TestCase(10), TestCase(100), TestCase(205)]
        public async Task Delete_IdNotFound(int nonExisting)
        {
            var fixture = new Fixture();
            var geneartor = fixture.Create<Generator<int>>();

            // Create fixtures, excluding the nonExisting integer from any possible Ids
            var mockCheckouts = fixture.Build<Checkout>()
                .Without(c => c.Id)
                .Do(x => x.Id = geneartor.Where(x => x != nonExisting).Take(1).FirstOrDefault())
                .CreateMany().ToList();

            contextMock.Setup(c => c.Checkouts)
                .Returns(DbSetProvider.CreateSet(mockCheckouts));

            // Searching for the nonexistant Id should throw an exception
            Func<Task> action = async () => await checkoutService.DeleteAsync(nonExisting);
            action.Should().Throw<ArgumentException>();
        }
        #endregion

        #region Create Tests
        [Test, TestCaseSource("checkoutRequestProvider")]
        public async Task Create_Valid(Checkout checkout)
        {
            // Empty list
            var checkoutList = new List<Checkout>();

            // Pass FindAsync params & return the entry found in the checkoutList
            var _mockSet = DbSetProvider.MockSet(checkoutList);
            _mockSet.Setup(s => s.FindAsync(It.IsAny<object[]>()))
                .ReturnsAsync((object[] p) => checkoutList.Find(c => c.Id == (int)p[0]));
            
            var mockSet = _mockSet.Object;
            // Setup the mock to be an empty set
            contextMock.Setup(c => c.Checkouts)
                .Returns(mockSet);

            var result = await checkoutService.CreateAsync(checkout);
            result.Should().BeEquivalentTo(checkout);
            mockSet.Count().Should().Equals(1);
        }
        /*
        [Test]
        public async Task Create_ItemUnavailable(ICheckoutRequest checkout)
        {
            Func<Task> action = async() => await checkoutService.CreateAsync(checkout);
            action.Should().Throw<ArgumentException>();
        }
        */
        #endregion
    }
}
