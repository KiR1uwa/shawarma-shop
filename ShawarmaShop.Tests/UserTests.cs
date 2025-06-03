using Microsoft.VisualStudio.TestTools.UnitTesting;
using Authentication;

namespace ShawarmaShop.Tests
{
    [TestClass]
    public class UserTests
    {
        [TestMethod]
        public void CreateUser_HasCorrectFields()
        {
            var user = new User
            {
                Id = 10,
                Username = "admin",
                PasswordHash = PasswordHasher.Hash("pass"),
                Role = UserRoles.Admin,
                Phone = "+48123456789",
                Email = "admin@example.com"
            };

            Assert.AreEqual("admin", user.Username);
            Assert.AreEqual(UserRoles.Admin, user.Role);
            Assert.AreEqual("+48123456789", user.Phone);
            Assert.AreEqual("admin@example.com", user.Email);
        }
    }
}
