using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using ShawarmaShopCore.Models;
using Microsoft.EntityFrameworkCore;
using ShawarmaShop.Infrastructure.Data;

namespace ShawarmaShop.Tests
{
    [TestClass]
    public class UserPanelWindowTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var db = new AppDbContext(options);

            db.Clients.Add(new Client { Name = "John", Phone = "+48000000000", Email = "a@b.com" });

            db.Shawarmas.Add(new Shawarma
            {
                Id = 1,
                Name = "Test",
                Price = 10.0m,
                Ingredients = "Test ingredients"
            });

            db.SaveChanges();

            return db;
        }

        [TestMethod]
        public void DeliveryAt_Should_BeTomorrow_IfTimePassed()
        {
            DateTime now = DateTime.Today.AddHours(22);
            DateTime selectedTime = new DateTime(now.Year, now.Month, now.Day, 21, 0, 0);

            DateTime expected = now.AddDays(1).Date.AddHours(21);
            DateTime actual = GetDeliveryAtAdjusted(now, selectedTime.TimeOfDay);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DeliveryAt_ShouldBeToday_IfTimeFuture()
        {
            DateTime now = DateTime.Today.AddHours(10);
            DateTime selectedTime = new DateTime(now.Year, now.Month, now.Day, 18, 0, 0);

            DateTime expected = now.Date.AddHours(18);
            DateTime actual = GetDeliveryAtAdjusted(now, selectedTime.TimeOfDay);

            Assert.AreEqual(expected, actual);
        }

        private DateTime GetDeliveryAtAdjusted(DateTime currentTime, TimeSpan selectedTime)
        {
            DateTime todayTime = currentTime.Date.Add(selectedTime);
            return todayTime > currentTime ? todayTime : todayTime.AddDays(1);
        }

        [TestMethod]
        public void TotalPrice_IsCalculatedCorrectly()
        {
            var items = new List<OrderItem>
            {
                new OrderItem { Quantity = 2, Shawarma = new Shawarma { Price = 10 } },
                new OrderItem { Quantity = 1, Shawarma = new Shawarma { Price = 5 } }
            };

            decimal total = items.Sum(i => i.Quantity * i.Shawarma.Price);

            Assert.AreEqual(25m, total);
        }

        [TestMethod]
        public void NewClient_IsCreated_IfNotExists()
        {
            using var db = GetDbContext();

            string phone = "+48111111111";
            string name = "Alice";
            string email = "alice@example.com";

            var client = db.Clients.FirstOrDefault(c => c.Phone == phone);

            if (client == null)
            {
                client = new Client { Name = name, Phone = phone, Email = email };
                db.Clients.Add(client);
                db.SaveChanges();
            }

            var found = db.Clients.FirstOrDefault(c => c.Phone == phone);

            Assert.IsNotNull(found);
            Assert.AreEqual(name, found.Name);
        }
    }
}
