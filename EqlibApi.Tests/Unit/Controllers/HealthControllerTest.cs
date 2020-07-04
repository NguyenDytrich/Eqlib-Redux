using NUnit.Framework;
using EqlibApi.Controllers;

namespace EqlibApi.Tests.Unit.Controllers
{
    public class HealthControllerTest
    {
        private HealthController controller;
        [SetUp]
        public void Setup()
        {
            controller = new HealthController();
        }

        [Test]
        public void PingTest()
        {
            Assert.AreEqual("pong", controller.Ping());
        }
    }
}