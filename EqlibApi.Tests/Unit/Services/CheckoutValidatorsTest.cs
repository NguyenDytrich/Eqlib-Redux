using AutoFixture;
using EqlibApi.Models;
using EqlibApi.Models.Db;
using EqlibApi.Models.Enums;
using EqlibApi.Services;
using EqlibApi.Tests.Unit.Utils;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EqlibApi.Tests.Unit.Services
{
    [Category("Unit")]
    class CheckoutValidatorsTest
    {
        private Mock<IApplicationContext> contextMock;
        private CheckoutValidators validator;
        private Fixture fixture;

        [SetUp]
        public void SetUp()
        {
            contextMock = new Mock<IApplicationContext>();
            validator = new CheckoutValidators(contextMock.Object);
            fixture = new Fixture();

            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Test]
        public void Item_Valid()
        {
            var item = new Item()
            {
                Id = 1,
                Availability = EAvailability.Available,
            };

            var itemSetMock = DbSetProvider.MockSet(new List<Item>() { item });
            contextMock.Setup(c => c.Items).Returns(itemSetMock.Object);
            itemSetMock.Setup(s => s.Find(It.IsAny<int>())).Returns(item);

            var result = validator.TestValidate(
                new Checkout()
                {
                    ItemIds = new List<int>() { item.Id },
                    CheckoutDate = DateTime.Now,
                    DueDate = DateTime.Now.AddDays(1)
                });
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test, TestCaseSource("InvalidItemProvider")]
        public void Item_Unavailable(Item item)
        {
            var items = new List<Item>() { item };
            var itemSetMock = DbSetProvider.MockSet(items);
            contextMock.Setup(c => c.Items).Returns(itemSetMock.Object);
            itemSetMock.Setup(s => s.Find(It.IsAny<int>()))
                .Returns((object[] i) => items.Find(c => c.Id == (int)i[0]));

            var result = validator.TestValidate(
                new Checkout()
                {
                    ItemIds = new List<int>() { item.Id }
                });

            result.ShouldHaveValidationErrorFor(c => c.ItemIds)
                .WithErrorMessage("Item specified by ItemId is not available.");
        }
        public static IEnumerable<TestCaseData> InvalidItemProvider()
        {
            yield return new TestCaseData(new Item() { Availability = EAvailability.Unavailable });
            yield return new TestCaseData(new Item() { Availability = EAvailability.CheckedOut });
            yield return new TestCaseData(new Item() { Availability = EAvailability.Hold });
            yield return new TestCaseData(new Item() { Availability = EAvailability.Lost });
        }

        [Test]
        public void Item_NotFound()
        {
            var items = new List<Item>();
            var itemSetMock = DbSetProvider.MockSet(items);
            contextMock.Setup(c => c.Items).Returns(itemSetMock.Object);
            itemSetMock.Setup(s => s.Find(It.IsAny<int>())).Returns<Item>(null);

            var result = validator.TestValidate(new Checkout()
            {
                ItemIds = new List<int>() { 0 },
            });

            result.ShouldHaveValidationErrorFor(c => c.ItemIds)
                .WithErrorMessage("Item specified by ItemId 0 does not exist.");
        }

        [Test]
        public void Item_NoItemsInItemIds()
        {
            var result = validator.TestValidate(new Checkout()
            {
                ItemIds = new List<int>()
            });

            result.ShouldHaveValidationErrorFor(c => c.ItemIds)
                .WithErrorMessage("Need at least 1 ItemId.");
        }

        [Test]
        public void DueDate_InvalidRange()
        {
            var checkout = fixture.Build<Checkout>()
                .With(c => c.CheckoutDate, DateTime.Now)
                .With(c => c.DueDate, DateTime.Now.AddDays(-1))
                .Create();
            var item = fixture.Build<Item>().With(i => i.Availability, EAvailability.Available).Create();

            var itemSetMock = DbSetProvider.MockSet(new List<Item>());
            contextMock.Setup(c => c.Items).Returns(itemSetMock.Object);
            itemSetMock.Setup(s => s.Find(It.IsAny<int>())).Returns((object[] i) => item);

            var result = validator.TestValidate(checkout);
            result.ShouldHaveValidationErrorFor(c => c.DueDate)
                .WithErrorMessage("Due date must be after checkout date.");
        }

        [Test]
        public void ReturnDate_InvalidRange()
        {
            var checkout = fixture.Build<Checkout>()
                .With(c => c.CheckoutDate, DateTime.Now)
                .With(c => c.DueDate, DateTime.Now.AddDays(1))
                .With(c => c.ReturnDate, DateTime.Now.AddDays(-1))
                .Create();
            var item = fixture.Build<Item>().With(i => i.Availability, EAvailability.Available).Create();

            var itemSetMock = DbSetProvider.MockSet(new List<Item>());
            contextMock.Setup(c => c.Items).Returns(itemSetMock.Object);
            itemSetMock.Setup(s => s.Find(It.IsAny<int>())).Returns((object[] i) => item);

            var result = validator.TestValidate(checkout);
            result.ShouldHaveValidationErrorFor(c => c.ReturnDate)
                .WithErrorMessage("Return date must be after checkout date.");
        }
    }
}
