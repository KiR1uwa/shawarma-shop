using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Shawarma_.Models;

namespace ShawarmaShop
{
    public partial class CartItemControl : UserControl
    {
        private readonly List<Shawarma> menu;
        private static readonly Regex digits = new(@"^\d+$");

        public decimal UnitPrice { get; private set; }
        public int Quantity { get; private set; } = 1;

        public Shawarma? SelectedDish => cmbDish.SelectedItem as Shawarma;

        public event EventHandler TotalsChanged;
        public event EventHandler DeleteRequested;

        public CartItemControl(List<Shawarma> menu)
        {
            InitializeComponent();
            this.menu = menu;
            cmbDish.ItemsSource = menu;
        }

        private void CmbDish_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbDish.SelectedItem is Shawarma dish)
            {
                UnitPrice = dish.Price;
                txtPrice.Text = $"{dish.Price:F2}";
                txtIngredients.Text = dish.Ingredients;
            }
            else
            {
                UnitPrice = 0;
                txtPrice.Text = txtIngredients.Text = "";
            }
            TotalsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void TxtQty_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) =>
            e.Handled = !digits.IsMatch(e.Text);

        private void TxtQty_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(txtQty.Text, out int q) && q > 0)
                Quantity = q;
            else
                Quantity = 0;
            TotalsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e) =>
            DeleteRequested?.Invoke(this, EventArgs.Empty);
    }
}
