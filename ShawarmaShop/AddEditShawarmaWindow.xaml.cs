using System;
using System.Globalization;
using System.Windows;
using Shawarma_.Models;

namespace ShawarmaShop
{
    public partial class AddEditShawarmaWindow : Window
    {
        public Shawarma Shawarma { get; set; }

        public AddEditShawarmaWindow()
        {
            InitializeComponent();
            Shawarma = new Shawarma();
            Title = "Add Shawarma";
        }

        public AddEditShawarmaWindow(Shawarma shawarma)
        {
            InitializeComponent();
            Shawarma = new Shawarma
            {
                Id = shawarma.Id,
                Name = shawarma.Name,
                Ingredients = shawarma.Ingredients,
                Price = shawarma.Price
            };
            Title = "Edit Shawarma";
            LoadShawarmaData();
        }

        private void LoadShawarmaData()
        {
            TxtName.Text = Shawarma.Name;
            TxtIngredients.Text = Shawarma.Ingredients;
            TxtPrice.Text = Shawarma.Price.ToString("F2");
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput())
            {
                Shawarma.Name = TxtName.Text.Trim();
                Shawarma.Ingredients = TxtIngredients.Text.Trim();
                Shawarma.Price = decimal.Parse(TxtPrice.Text);

                DialogResult = true;
                Close();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                MessageBox.Show("Please enter a name for the shawarma.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtIngredients.Text))
            {
                MessageBox.Show("Please enter ingredients for the shawarma.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtIngredients.Focus();
                return false;
            }

            if (!decimal.TryParse(TxtPrice.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Please enter a valid price greater than 0.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtPrice.Focus();
                return false;
            }

            return true;
        }
    }
}