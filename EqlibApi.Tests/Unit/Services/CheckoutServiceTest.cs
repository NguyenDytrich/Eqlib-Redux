using AutoFixture;
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
using System.Text;
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
        #endregion

        [SetUp]
        protected void SetUp()
        {
            contextMock = new Mock<IApplicationContext>();
            checkoutService = new CheckoutService(contextMock.Object);
        }

        [Test, TestCaseSource("checkoutsProvider")]
        public async Task GetAsync_Valid(List<Checkout> checkouts)
        {
            contextMock.Setup(c => c.Checkouts).Returns(MockDbContext.CreateSet(checkouts));

            var result = await checkoutService.GetAsync();
            result.Should().BeEquivalentTo(checkouts);
        }

    }
}
