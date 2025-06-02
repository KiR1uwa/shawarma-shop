using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShawarmaShopCore.Validation;

namespace ShawarmaShop.Tests.Validation
{
    [TestClass]
    public class ClientValidatorTests
    {
        [TestMethod]
        public void IsValidPhone_ValidPlainNumber_ReturnsTrue()
        {
            Assert.IsTrue(ClientValidator.IsValidPhone("1234567890"));
        }

        [TestMethod]
        public void IsValidPhone_ValidWithPlus_ReturnsTrue()
        {
            Assert.IsTrue(ClientValidator.IsValidPhone("+48123456789"));
        }

        [TestMethod]
        public void IsValidPhone_ValidWithSpaces_ReturnsTrue()
        {
            Assert.IsTrue(ClientValidator.IsValidPhone("+48 123 456 789"));
        }

        [TestMethod]
        public void IsValidPhone_TooShort_ReturnsFalse()
        {
            Assert.IsFalse(ClientValidator.IsValidPhone("12345"));
        }

        [TestMethod]
        public void IsValidPhone_Empty_ReturnsFalse()
        {
            Assert.IsFalse(ClientValidator.IsValidPhone(""));
        }

        [TestMethod]
        public void IsValidPhone_Null_ReturnsFalse()
        {
            Assert.IsFalse(ClientValidator.IsValidPhone(null));
        }

        [TestMethod]
        public void IsValidPhone_WithLetters_ReturnsFalse()
        {
            Assert.IsFalse(ClientValidator.IsValidPhone("12345abcde"));
        }


        [TestMethod]
        public void IsValidEmail_ValidEmail_ReturnsTrue()
        {
            Assert.IsTrue(ClientValidator.IsValidEmail("test@example.com"));
        }

        [TestMethod]
        public void IsValidEmail_MissingAtSymbol_ReturnsFalse()
        {
            Assert.IsFalse(ClientValidator.IsValidEmail("testexample.com"));
        }

        [TestMethod]
        public void IsValidEmail_Empty_ReturnsFalse()
        {
            Assert.IsFalse(ClientValidator.IsValidEmail(""));
        }

        [TestMethod]
        public void IsValidEmail_Null_ReturnsFalse()
        {
            Assert.IsFalse(ClientValidator.IsValidEmail(null));
        }

        [TestMethod]
        public void IsValidName_ValidName_ReturnsTrue()
        {
            Assert.IsTrue(ClientValidator.IsValidName("Anna"));
        }

        [TestMethod]
        public void IsValidName_TooShort_ReturnsFalse()
        {
            Assert.IsFalse(ClientValidator.IsValidName("A"));
        }

        [TestMethod]
        public void IsValidName_Empty_ReturnsFalse()
        {
            Assert.IsFalse(ClientValidator.IsValidName(""));
        }

        [TestMethod]
        public void IsValidName_Null_ReturnsFalse()
        {
            Assert.IsFalse(ClientValidator.IsValidName(null));
        }
    }
}
