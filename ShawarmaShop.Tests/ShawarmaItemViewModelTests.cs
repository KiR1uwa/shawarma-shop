using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using Shawarma_.Models;
using System;

namespace ShawarmaShop.Tests
{
    [TestClass]
    public class ShawarmaItemViewModelTests
    {
        private class ShawarmaItemViewModel : INotifyPropertyChanged
        {
            public int Id { get; }
            public string Name { get; }
            public decimal Price { get; }
            private bool isSelected;
            private int quantity = 1;

            public ShawarmaItemViewModel(Shawarma s)
            {
                Id = s.Id;
                Name = s.Name;
                Price = s.Price;
            }

            public bool IsSelected
            {
                get => isSelected;
                set { isSelected = value; OnChanged(); }
            }

            public int Quantity
            {
                get => quantity;
                set { if (value > 0) { quantity = value; OnChanged(); } }
            }

            public decimal Subtotal => IsSelected ? Price * Quantity : 0;

            public event PropertyChangedEventHandler? PropertyChanged;
            private void OnChanged(string? prop = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        [TestMethod]
        public void Subtotal_ShouldBeCorrect_WhenSelected()
        {
            var vm = new ShawarmaItemViewModel(new Shawarma { Id = 1, Name = "Test", Price = 10m })
            {
                IsSelected = true,
                Quantity = 2
            };
            Assert.AreEqual(20m, vm.Subtotal);
        }

        [TestMethod]
        public void Subtotal_ShouldBeZero_WhenNotSelected()
        {
            var vm = new ShawarmaItemViewModel(new Shawarma { Id = 2, Name = "Veg", Price = 15m })
            {
                IsSelected = false,
                Quantity = 3
            };
            Assert.AreEqual(0m, vm.Subtotal);
        }

        [TestMethod]
        public void Quantity_ShouldIgnoreNegative()
        {
            var vm = new ShawarmaItemViewModel(new Shawarma { Id = 3, Name = "Falafel", Price = 12m });
            vm.Quantity = -4;
            Assert.AreEqual(1, vm.Quantity);
        }
    }
}
