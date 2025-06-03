using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ShawarmaShop.Tests
{
    [TestClass]
    public class OrderWindowValidationTests
    {
        private bool Validate(string? client, DateTime? date, string? time, List<(bool selected, int qty)> items)
        {
            if (string.IsNullOrEmpty(client)) return false;
            if (!date.HasValue) return false;
            if (!TimeSpan.TryParseExact(time, "hh\\:mm", CultureInfo.InvariantCulture, out _)) return false;
            if (!items.Any(x => x.selected && x.qty > 0)) return false;
            return true;
        }

        [TestMethod]
        public void Validate_WithCorrectData_ReturnsTrue()
        {
            var result = Validate("John", DateTime.Today, "15:00", new() { (true, 2) });
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Validate_WithNoClient_ReturnsFalse()
        {
            var result = Validate(null, DateTime.Today, "12:00", new() { (true, 1) });
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Validate_WithInvalidTime_ReturnsFalse()
        {
            var result = Validate("Jane", DateTime.Today, "notTime", new() { (true, 1) });
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Validate_WithNoItemsSelected_ReturnsFalse()
        {
            var result = Validate("Anna", DateTime.Today, "13:00", new() { (false, 0), (false, 0) });
            Assert.IsFalse(result);
        }
    }
}
