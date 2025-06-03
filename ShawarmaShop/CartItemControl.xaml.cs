// =====================================
// File: CartItemControl.xaml.cs
// =====================================

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using ShawarmaShopCore.Models;

namespace ShawarmaShop
{
    /// <summary>
    /// UI control that represents a single item row within the shopping cart.
    /// </summary>
    public partial class CartItemControl : UserControl
    {
        /// <summary>
        /// Cached reference to the full menu used to populate the combo box.
        /// </summary>
        private readonly List<Shawarma> menu;

        /// <summary>
        /// Regular expression used to allow only digits in the quantity textbox.
        /// </summary>
        private static readonly Regex digits = new(@"^\d+$");

        /// <summary>
        /// Gets the price of the currently selected dish.
        /// </summary>
        public decimal UnitPrice { get; private set; }

        /// <summary>
        /// Gets the quantity entered by the user. Defaults to <c>1</c>.
        /// </summary>
        public int Quantity { get; private set; } = 1;

        /// <summary>
        /// Gets the dish chosen by the user or <see langword="null"/> if none is selected.
        /// </summary>
        public Shawarma? SelectedDish => cmbDish.SelectedItem as Shawarma;

        /// <summary>
        /// Raised whenever <see cref="UnitPrice"/> or <see cref="Quantity"/> changes,
        /// allowing the parent control to recalculate totals.
        /// </summary>
        public event EventHandler? TotalsChanged;

        /// <summary>
        /// Raised when the user requests deletion of the item (via the delete button).
        /// </summary>
        public event EventHandler? DeleteRequested;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartItemControl"/> class.
        /// </summary>
        /// <param name="menu">Collection of available dishes to choose from.</param>
        public CartItemControl(List<Shawarma> menu)
        {
            InitializeComponent();
            this.menu = menu;
            cmbDish.ItemsSource = menu;
        }

        /// <summary>
        /// Handles selection changes in the dish combo box – updates internal state and UI.
        /// </summary>
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
                txtPrice.Text = txtIngredients.Text = string.Empty;
            }
            TotalsChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Ensures that only digits can be entered in the quantity textbox.
        /// </summary>
        private void TxtQty_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) =>
            e.Handled = !digits.IsMatch(e.Text);

        /// <summary>
        /// Tracks quantity changes and raises <see cref="TotalsChanged"/> as appropriate.
        /// </summary>
        private void TxtQty_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(txtQty.Text, out int q) && q > 0)
                Quantity = q;
            else
                Quantity = 0;
            TotalsChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Forwards a delete request to any subscribers.
        /// </summary>
        private void BtnDelete_Click(object sender, RoutedEventArgs e) =>
            DeleteRequested?.Invoke(this, EventArgs.Empty);
    }
}