using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ShawarmaShop.Tests
{
    [TestClass]
    public class OrderWindowRecalcTotalTests
    {
        private class ShawarmaItemViewModel : INotifyPropertyChanged
        {
            public int Id { get; }
            public string Name { get; }
            public decimal Price { get; }
            private bool isSelected;
            private int quantity = 1;

            public ShawarmaItemViewModel(int id, string name, decimal price)
            {
                Id = id;
                Name = name;
                Price = price;
            }

            public bool IsSelected
            {
                get => isSelected;
                set { isSelected = value; OnChanged(); }
            }

            public int Quantity
            {
                get => quantity;
                set { quantity = value; }
            }

            public decimal Subtotal => IsSelected && Quantity > 0 ? Price * Quantity : 0;

            public event PropertyChangedEventHandler? PropertyChanged;
            private void OnChanged(string? prop = null) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private decimal CalculateTotal(List<ShawarmaItemViewModel> items)
        {
            return items.Where(x => x.IsSelected && x.Quantity > 0).Sum(x => x.Subtotal);
        }

        [TestMethod]
        public void RecalcTotal_WithSelectedItems_ReturnsCorrectSum()
        {
            var items = new List<ShawarmaItemViewModel>
            {
                new ShawarmaItemViewModel(1, "A", 10m) { IsSelected = true, Quantity = 2 },
                new ShawarmaItemViewModel(2, "B", 15m) { IsSelected = true, Quantity = 1 },
                new ShawarmaItemViewModel(3, "C", 5m)  { IsSelected = false, Quantity = 5 }
            };

            decimal total = CalculateTotal(items);

            Assert.AreEqual(35m, total);
        }

        [TestMethod]
        public void RecalcTotal_WithNoSelectedItems_ReturnsZero()
        {
            var items = new List<ShawarmaItemViewModel>
            {
                new ShawarmaItemViewModel(1, "A", 10m) { IsSelected = false, Quantity = 2 },
                new ShawarmaItemViewModel(2, "B", 15m) { IsSelected = false, Quantity = 1 }
            };

            decimal total = CalculateTotal(items);

            Assert.AreEqual(0m, total);
        }

        [TestMethod]
        public void RecalcTotal_WithZeroQuantity_ReturnsZero()
        {
            var items = new List<ShawarmaItemViewModel>
            {
                new ShawarmaItemViewModel(1, "A", 10m) { IsSelected = true, Quantity = 0 },
                new ShawarmaItemViewModel(2, "B", 15m) { IsSelected = true, Quantity = 0 }
            };

            decimal total = CalculateTotal(items);

            Assert.AreEqual(0m, total);
        }
    }
}
