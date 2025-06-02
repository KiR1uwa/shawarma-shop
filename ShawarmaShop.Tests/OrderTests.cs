using Microsoft.VisualStudio.TestTools.UnitTesting;
using Order_.Models;
using OrderItem_.Models;
using Shawarma_.Models;
using Client_.Models;
using System;

namespace ShawarmaShop.Tests
{
    [TestClass]
    public class OrderTests
    {
        [TestMethod]
        public void Order_InitializesCorrectly()
        {
            var order = new Order
            {
                Id = 1,
                CreatedAt = DateTime.Today,
                DeliveryAt = DateTime.Today.AddHours(2),
                Comment = "No onions",
                ClientID = 3,
                Client = new Client { Name = "Jane" }
            };

            Assert.AreEqual(1, order.Id);
            Assert.AreEqual("Jane", order.Client.Name);
            Assert.AreEqual("No onions", order.Comment);
        }
    }

    [TestClass]
    public class OrderItemTests
    {
        [TestMethod]
        public void OrderItem_Creation_SetsFields()
        {
            var item = new OrderItem
            {
                Id = 7,
                OrderId = 3,
                ShawarmaId = 2,
                Quantity = 5
            };

            Assert.AreEqual(7, item.Id);
            Assert.AreEqual(3, item.OrderId);
            Assert.AreEqual(2, item.ShawarmaId);
            Assert.AreEqual(5, item.Quantity);
        }
    }
}
