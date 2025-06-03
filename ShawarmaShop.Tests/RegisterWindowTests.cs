using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace ShawarmaShop.Tests
{
    [TestClass]
    public class RegisterWindowTests
    {
        private bool ValidateRegistration(string username, string password, string phone, string email)
        {
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(email))
                return false;

            var phoneRx = new Regex(@"^\+?[1-9]\d{8,14}$");
            if (!phoneRx.IsMatch(phone))
                return false;

            var emailRx = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!emailRx.IsMatch(email))
                return false;

            return true;
        }

        [TestMethod]
        public void Register_WithAllValidFields_ReturnsTrue()
        {
            var result = ValidateRegistration("user1", "pass123", "+48123456789", "user1@mail.com");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Register_WithEmptyFields_ReturnsFalse()
        {
            var result = ValidateRegistration("", "pass123", "+48123456789", "user1@mail.com");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Register_WithInvalidPhone_ReturnsFalse()
        {
            var result = ValidateRegistration("user1", "pass123", "12345", "user1@mail.com");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Register_WithInvalidEmail_ReturnsFalse()
        {
            var result = ValidateRegistration("user1", "pass123", "+48123456789", "user1mail.com");
            Assert.IsFalse(result);
        }
    }
}
