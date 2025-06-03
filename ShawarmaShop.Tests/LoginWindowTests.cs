using Microsoft.VisualStudio.TestTools.UnitTesting;
using Authentication;
using System.Collections.Generic;
using System.Linq;

namespace ShawarmaShop.Tests
{
    [TestClass]
    public class LoginWindowTests
    {
        private List<User> mockUsers = new List<User>
        {
            new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = PasswordHasher.Hash("admin123"),
                Role = "admin"
            },
            new User
            {
                Id = 2,
                Username = "user",
                PasswordHash = PasswordHasher.Hash("userpass"),
                Role = "user"
            }
        };

        private User? TryLogin(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return null;

            var user = mockUsers.FirstOrDefault(u => u.Username == username);
            if (user != null && PasswordHasher.Verify(password, user.PasswordHash))
                return user;

            return null;
        }

        [TestMethod]
        public void Login_WithCorrectCredentials_ReturnsUser()
        {
            var user = TryLogin("admin", "admin123");
            Assert.IsNotNull(user);
            Assert.AreEqual("admin", user!.Username);
        }

        [TestMethod]
        public void Login_WithIncorrectPassword_ReturnsNull()
        {
            var user = TryLogin("admin", "wrongpass");
            Assert.IsNull(user);
        }

        [TestMethod]
        public void Login_WithNonexistentUser_ReturnsNull()
        {
            var user = TryLogin("ghost", "123");
            Assert.IsNull(user);
        }

        [TestMethod]
        public void Login_WithEmptyFields_ReturnsNull()
        {
            var user = TryLogin("", "");
            Assert.IsNull(user);
        }
    }
}
