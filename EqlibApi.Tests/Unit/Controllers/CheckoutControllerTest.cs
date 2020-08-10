using EqlibApi.Models;
using EqlibApi.Models.Db;
using EqlibApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace EqlibApi.Tests.Unit.Controllers
{
    class CheckoutControllerTest
    {
        private CheckoutController controller;
        private ApplicationContext context;
        
        [Test]
        /// <summary>
        /// Test for a valid GET request
        /// </summary>
        public async Task Get()
        {
            var result = await controller.GetCheckouts();
            Assert.IsInstanceOf<IEnumerable<Checkout>>(result.Value);
            Assert.AreEqual(context.Checkouts.Count(), result.Value.Count());
        }

        [Test]
        /// <summary>
        /// GET a valid checkout by Id
        /// </summary>
        public async Task GetById()
        {
            var result = await controller.GetCheckouts(1);
            Assert.IsInstanceOf<Checkout>(result.Value);
            Assert.AreEqual(1, result.Value.Id);
        }

        [Test]
        /// <summary>
        /// GET a checkout by Id that does not exist
        /// </summary>
        public async Task NotFoundGetId()
        {
            var result = await controller.GetCheckouts(2000);
            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }
    }
}
