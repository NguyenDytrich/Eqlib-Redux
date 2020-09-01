using EqlibApi.Models;
using EqlibApi.Models.Db;
using EqlibApi.Models.Enums;
using EqlibApi.Services;
using EqlibApi.Tests.Unit.Utils;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace EqlibApi.Tests.Unit.Services
{
    [Category("Unit")]
    class CheckoutValidatorsTest
    {
        private Mock<IApplicationContext> contextMock;
        private CheckoutValidators validator;

        [SetUp]
        public void SetUp()
        {
            contextMock = new Mock<IApplicationContext>();
            validator = new CheckoutValidators(contextMock.Object);

        }

        [Test]
        public void Item_Valid()
        {
            var item = new Item()
            {
                Id = 1,
                Availability = EAvailability.Available
            };

            var itemSetMock = DbSetProvider.MockSet(new List<Item>() { item });
            contextMock.Setup(c => c.Items).Returns(itemSetMock.Object);
            itemSetMock.Setup(s => s.Find(It.IsAny<int>())).Returns(item);

            var result = validator.TestValidate(
                new Checkout()
                {
                    ItemIds = new List<int>() { item.Id }
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
                .WithErrorMessage("Item specified by ItemId is not Available.");
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
                ItemIds = new List<int>() { 0 }
            });

            result.ShouldHaveValidationErrorFor(c => c.ItemIds)
                .WithErrorMessage("Item specified by ItemId does not exist.");
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
    }
}
