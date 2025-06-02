using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShawarmaShopCore.Validation;

namespace ShawarmaShop.Tests.Validation
{
    [TestClass]
    public class ClientValidatorTests
    {
        [TestMethod]
        public void IsValidPhone_ValidNumber_ReturnsTrue()
        {
            Assert.IsTrue(ClientValidator.IsValidPhone("1234567890"));
        }

        [TestMethod]
        public void IsValidPhone_InvalidLength_ReturnsFalse()
        {
            Assert.IsFalse(ClientValidator.IsValidPhone("12345"));
        }

        [TestMethod]
        public void IsValidPhone_NonDigitCharacters_ReturnsFalse()
        {
            Assert.IsFalse(ClientValidator.IsValidPhone("12345abcde"));
        }

        [TestMethod]
        public void IsValidEmail_ValidFormat_ReturnsTrue()
        {
            Assert.IsTrue(ClientValidator.IsValidEmail("test@example.com"));
        }

        [TestMethod]
        public void IsValidEmail_MissingAtSymbol_ReturnsFalse()
        {
            Assert.IsFalse(ClientValidator.IsValidEmail("testexample.com"));
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
    }
}
