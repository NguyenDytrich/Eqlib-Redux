﻿using AutoFixture;
using EqlibApi.Controllers;
using EqlibApi.Models.Db;
using EqlibApi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EqlibApi.Tests.Unit.Controllers
{
    class CheckoutControllerTest
    {
        private CheckoutController controller;
        private Mock<ICheckoutService> serviceMock;

        public static IEnumerable<TestCaseData> checkoutProvider()
        {
            var fixture = new Fixture();

            yield return new TestCaseData(fixture.Build<Checkout>()
                .Without(o => o.Items).Create());

            // Case for a Checkout object with multiple item Ids
            yield return new TestCaseData(fixture.Build<Checkout>()
                .Without(o => o.Items)
                .Do(o =>
                {
                    o.ItemIds = fixture.CreateMany<int>();
                }).Create());
        }

        [SetUp]
        protected void SetUp()
        {
            serviceMock = new Mock<ICheckoutService>();
            controller = new CheckoutController(serviceMock.Object);
        }

        #region Get Tests
        [Test]
        /// <summary>
        /// Test for a valid GET request
        /// </summary>
        public async Task Get_Valid()
        {
            var fixture = new Fixture();
            var checkouts = fixture.CreateMany<Checkout>();
            serviceMock.Setup(s => s.GetAsync()).ReturnsAsync(checkouts.ToList());

            var result = await controller.GetCheckouts();
            Assert.IsInstanceOf<IEnumerable<Checkout>>(result.Value);
            Assert.AreEqual(checkouts.Count(), result.Value.Count());
        }

        [Test]
        /// <summary>
        /// GET a valid checkout by Id
        /// </summary>
        public async Task Get_ById()
        {
            var fixture = new Fixture();
            var checkout = new List<Checkout> { fixture.Create<Checkout>() };
            serviceMock.Setup(s => s.GetAsync(It.IsAny<Expression<Func<Checkout, bool>>>())).ReturnsAsync(checkout);

            var result = await controller.GetCheckouts(checkout[0].Id);
            Assert.IsInstanceOf<Checkout>(result.Value);
            Assert.AreEqual(checkout[0].Id, result.Value.Id);
        }

        [Test]
        /// <summary>
        /// GET a checkout by Id that does not exist
        /// </summary>
        public async Task Get_NotFound()
        {
            serviceMock.Setup(s => s.GetAsync(It.IsAny<Expression<Func<Checkout, bool>>>())).ReturnsAsync(new List<Checkout>());
            var result = await controller.GetCheckouts(2000);
            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }
        #endregion

        #region Delete Tests
        [Test]
        /// <summary>
        /// Test for a valid DELETE request
        /// </summary>
        public async Task Delete_Valid()
        {
            serviceMock.Setup(s => s.CheckoutExists(It.IsAny<int>())).Returns(true);
            var response = await controller.DeleteCheckout(1);
            Assert.IsInstanceOf<NoContentResult>(response);
        }

        [Test]
        /// <summary>
        /// Test for a non-existant Id DELETE request
        /// </summary>
        public async Task Delete_NotFound()
        {
            serviceMock.Setup(s => s.CheckoutExists(It.IsAny<int>())).Returns(false);
            var response = await controller.DeleteCheckout(1);
            Assert.IsInstanceOf<NotFoundResult>(response);
        }
        #endregion

        #region Post Tests
        [Test, TestCaseSource("checkoutProvider")]
        /// <summary>
        /// Test for a valid POST request
        /// </summary>
        public async Task Post_Valid(Checkout checkout)
        {   
            // The service succesfully creates and returns the checkoutentry requested.
            serviceMock.Setup(s => s.CreateAsync(It.Is<Checkout>(c => c == checkout)))
                .ReturnsAsync(checkout);

            var result = await controller.PostCheckout(checkout);
            Assert.IsInstanceOf<Checkout>(result.Value);
        }

        [Test, TestCaseSource("checkoutProvider")]
        /// <summary>
        /// Test for a Checkout with invalid ItemIds
        /// </summary>
        public async Task Post_BadItemIds(Checkout checkout)
        {
            serviceMock.Setup(s => s.CreateAsync(It.IsAny<Checkout>()))
                .Throws(new ArgumentException());

            var result = await controller.PostCheckout(checkout);
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);

            // Create method should not be called
            serviceMock.Verify(s => s.CreateAsync(It.IsAny<Checkout>()), Times.Once);
        }
        #endregion
    }
}
