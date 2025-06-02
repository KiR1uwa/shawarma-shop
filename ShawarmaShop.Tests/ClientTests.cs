using Microsoft.VisualStudio.TestTools.UnitTesting;
using Client_.Models;

namespace ShawarmaShop.Tests
{
    [TestClass]
    public class ClientTests
    {
        [TestMethod]
        public void Client_Creation_WorksCorrectly()
        {
            var client = new Client
            {
                Id = 5,
                Name = "John",
                Phone = "+48777888999",
                Email = "john@example.com"
            };

            Assert.AreEqual("John", client.Name);
            Assert.AreEqual("+48777888999", client.Phone);
            Assert.AreEqual("john@example.com", client.Email);
            Assert.AreEqual(0, client.Orders.Count);
        }
    }
}
