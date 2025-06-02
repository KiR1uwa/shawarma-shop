using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shawarma_.Models;

namespace ShawarmaShop.Tests
{
    [TestClass]
    public class ShawarmaTests
    {
        [TestMethod]
        public void Shawarma_Constructor_SetsPropertiesCorrectly()
        {
            var s = new Shawarma
            {
                Id = 1,
                Name = "Beef",
                Ingredients = "Beef, Lettuce",
                Price = 20.5m
            };

            Assert.AreEqual(1, s.Id);
            Assert.AreEqual("Beef", s.Name);
            Assert.AreEqual("Beef, Lettuce", s.Ingredients);
            Assert.AreEqual(20.5m, s.Price);
        }
    }
}
