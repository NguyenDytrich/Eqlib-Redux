using AutoFixture;
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
        public async Task Get()
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
        public async Task GetById()
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
        public async Task NotFoundGetId()
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
        public async Task Delete()
        {
            serviceMock.Setup(s => s.IdExists(It.IsAny<int>())).Returns(true);
            var response = await controller.DeleteCheckout(1);
            Assert.IsInstanceOf<NoContentResult>(response);
        }

        [Test]
        /// <summary>
        /// Test for a non-existant Id DELETE request
        /// </summary>
        public async Task NotFoundDelete()
        {
            serviceMock.Setup(s => s.IdExists(It.IsAny<int>())).Returns(false);
            var response = await controller.DeleteCheckout(1);
            Assert.IsInstanceOf<NotFoundResult>(response);
        }
        #endregion
    }
}
