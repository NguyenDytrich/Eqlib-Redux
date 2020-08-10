using EqlibApi.Models.Db;
using EqlibApi.Controllers;
using EqlibApi.Services;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Moq;
using AutoFixture;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using System.Linq.Expressions;

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
    }
}
