using EqlibApi.Controllers;
using NUnit.Framework;

namespace EqlibApi.Tests.Unit.Controllers
{
    [Category("Unit")]
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