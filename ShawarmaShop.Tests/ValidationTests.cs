using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace ShawarmaShop.Tests
{
    [TestClass]
    public class ValidationTests
    {
        private bool IsValidPhone(string phone)
        {
            return Regex.IsMatch(phone, @"^\+?[1-9]\d{8,14}$");
        }

        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        [TestMethod]
        [DataRow("+48123456789", true)]
        [DataRow("123456789", true)]
        [DataRow("+48abc", false)]
        [DataRow("", false)]
        public void IsValidPhone_Tests(string input, bool expected)
        {
            Assert.AreEqual(expected, IsValidPhone(input));
        }

        [TestMethod]
        [DataRow("test@example.com", true)]
        [DataRow("test@site", false)]
        [DataRow("user@.com", false)]
        [DataRow("", false)]
        public void IsValidEmail_Tests(string input, bool expected)
        {
            Assert.AreEqual(expected, IsValidEmail(input));
        }
    }
}
