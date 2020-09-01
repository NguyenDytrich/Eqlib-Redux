using AutoFixture;
using EqlibApi.Models;
using EqlibApi.Models.Db;
using EqlibApi.Services;
using EqlibApi.Tests.Unit.Utils;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EqlibApi.Tests.Unit.Services
{
    [Category("Unit")]
    class CheckoutServiceTest
    {
        private CheckoutService checkoutService;
        private Mock<CheckoutValidators> validatorMock;
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
            validatorMock = new Mock<CheckoutValidators>(contextMock.Object);
            checkoutService = new CheckoutService(contextMock.Object, validatorMock.Object);
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
        public void Delete_IdNotFound(int nonExisting)
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
            // Create a mock DbSet from an empty List
            var checkoutList = new List<Checkout>();
            var checkoutSetMock = DbSetProvider.MockSet(checkoutList);

            // When FindAsync is called with any Int...
            checkoutSetMock.Setup(s => s.FindAsync(It.IsAny<int>()))
            // Box the the integer for the function call
            // then aynchrounously return a LINQ Find by unboxing p[0]
                .ReturnsAsync((object[] p) => checkoutList.Find(c => c.Id == (int)p[0]));

            // Return the DbSet from the mockSet.
            // The contextMock.Checkouts returns mockSet, which
            // in turn proxies checkoutList.Add() to
            // DbSet.AddAsync().
            var mockSet = checkoutSetMock.Object;
            contextMock.Setup(c => c.Checkouts)
                .Returns(mockSet);

            // Validator returns no rerrors
            validatorMock.Setup(v => v.Validate(It.IsAny<ValidationContext<Checkout>>()))
                .Returns(new ValidationResult());


            var result = await checkoutService.CreateAsync(checkout);
            result.Should().BeEquivalentTo(checkout);
            mockSet.Count().Should().Equals(1);
        }

        [Test]
        public void Create_ItemNotFound()
        {
            var fixture = new Fixture();
            var checkout = fixture.Create<Checkout>();

            validatorMock.Setup(v => v.Validate(It.IsAny<ValidationContext<Checkout>>()))
                .Returns(new ValidationResult(
                    new List<ValidationFailure>()
                    {
                        new ValidationFailure("ItemIds", "Item specified by ItemId does not exist.")
                    }));

            Func<Task> action = async () => await checkoutService.CreateAsync(checkout);
            action.Should().Throw<ArgumentException>();
        }

        [Test]
        public void Create_ItemNotAvailable()
        {
            var fixture = new Fixture();
            var checkout = fixture.Create<Checkout>();

            validatorMock.Setup(v => v.Validate(It.IsAny<ValidationContext<Checkout>>()))
                .Returns(new ValidationResult(
                    new List<ValidationFailure>()
                    {
                        new ValidationFailure("ItemIds", "Item specified by ItemId is not Available.")
                    }));

            Func<Task> action = async () => await checkoutService.CreateAsync(checkout);
            action.Should().Throw<ArgumentException>();
        }
        #endregion
    }
}
