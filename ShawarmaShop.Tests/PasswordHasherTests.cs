using Microsoft.VisualStudio.TestTools.UnitTesting;
using Authentication;

namespace ShawarmaShop.Tests
{
    [TestClass]
    public class PasswordHasherTests
    {
        [TestMethod]
        public void Hash_SamePassword_ReturnsSameHash()
        {
            string password = "12345";
            string hash1 = PasswordHasher.Hash(password);
            string hash2 = PasswordHasher.Hash(password);

            Assert.AreEqual(hash1, hash2);
        }

        [TestMethod]
        public void Verify_CorrectPassword_ReturnsTrue()
        {
            string password = "MySecret";
            string hash = PasswordHasher.Hash(password);

            Assert.IsTrue(PasswordHasher.Verify(password, hash));
        }

        [TestMethod]
        public void Verify_WrongPassword_ReturnsFalse()
        {
            string password = "Correct";
            string hash = PasswordHasher.Hash(password);

            Assert.IsFalse(PasswordHasher.Verify("Wrong", hash));
        }
    }
}
