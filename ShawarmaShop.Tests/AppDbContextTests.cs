using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Shawarma_.Models;
using Client_.Models;
using Order_.Models;
using OrderItem_.Models;

namespace ShawarmaShop.Tests
{
    [TestClass]
    public class AppDbContextTests
    {
        private DbContextOptions<AppDbContext> CreateOptions(string dbName)
        {
            return new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
        }

        [TestMethod]
        public void CanInsertClientIntoDatabase()
        {
            var options = CreateOptions("InsertClientDb");

            using (var context = new AppDbContext(options))
            {
                context.Clients.Add(new Client { Name = "Test", Phone = "+48123456789", Email = "test@mail.com" });
                context.SaveChanges();
            }

            using (var context = new AppDbContext(options))
            {
                var client = context.Clients.FirstOrDefault();
                Assert.IsNotNull(client);
                Assert.AreEqual("Test", client!.Name);
            }
        }

        [TestMethod]
        public void CanLinkOrderToClient()
        {
            var options = CreateOptions("LinkOrderDb");

            using (var context = new AppDbContext(options))
            {
                var client = new Client { Name = "Anna", Phone = "+48777777777", Email = "anna@mail.com" };
                context.Clients.Add(client);
                context.SaveChanges();

                var order = new Order
                {
                    CreatedAt = DateTime.Now,
                    DeliveryAt = DateTime.Now.AddHours(1),
                    ClientID = client.Id
                };
                context.Orders.Add(order);
                context.SaveChanges();
            }

            using (var context = new AppDbContext(options))
            {
                var order = context.Orders.Include(o => o.Client).FirstOrDefault();
                Assert.IsNotNull(order);
                Assert.IsNotNull(order!.Client);
                Assert.AreEqual("Anna", order.Client.Name);
            }
        }

        [TestMethod]
        public void CanAddShawarmaAndOrderItem()
        {
            var options = CreateOptions("ShawarmaOrderDb");

            using (var context = new AppDbContext(options))
            {
                var shawarma = new Shawarma { Name = "Beef", Ingredients = "Beef, Lettuce", Price = 20.0m };
                context.Shawarmas.Add(shawarma);
                context.SaveChanges();

                var order = new Order { CreatedAt = DateTime.Now, DeliveryAt = DateTime.Now.AddMinutes(30), ClientID = 1 };
                context.Orders.Add(order);
                context.SaveChanges();

                var item = new OrderItem
                {
                    OrderId = order.Id,
                    ShawarmaId = shawarma.Id,
                    Quantity = 2
                };
                context.OrderItems.Add(item);
                context.SaveChanges();
            }

            using (var context = new AppDbContext(options))
            {
                var item = context.OrderItems.Include(i => i.Shawarma).Include(i => i.Order).FirstOrDefault();
                Assert.IsNotNull(item);
                Assert.AreEqual(2, item!.Quantity);
                Assert.AreEqual("Beef", item.Shawarma.Name);
            }
        }
    }
}
