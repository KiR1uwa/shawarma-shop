// =====================================
// File: AddEditShawarmaWindow.xaml.cs
// =====================================

using System.Windows;
using ShawarmaShopCore.Models;

namespace ShawarmaShop
{
    /// <summary>
    /// Interaction logic for <c>AddEditShawarmaWindow</c>.
    /// Allows users to add a new <see cref="Shawarma"/> item or edit an existing one.
    /// </summary>
    public partial class AddEditShawarmaWindow : Window
    {
        /// <summary>
        /// Gets or sets the <see cref="Shawarma"/> instance being created or edited.
        /// </summary>
        public Shawarma Shawarma { get; set; }

        /// <summary>
        /// Initializes the window in <em>add‑new</em> mode.
        /// </summary>
        public AddEditShawarmaWindow()
        {
            InitializeComponent();
            Shawarma = new Shawarma();
            Title = "Add Shawarma";
        }

        /// <summary>
        /// Initializes the window in <em>edit</em> mode using the provided shawarma.
        /// </summary>
        /// <param name="shawarma">The shawarma to edit.</param>
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

        /// <summary>
        /// Loads the existing shawarma data into UI controls.
        /// </summary>
        private void LoadShawarmaData()
        {
            TxtName.Text = Shawarma.Name;
            TxtIngredients.Text = Shawarma.Ingredients;
            TxtPrice.Text = Shawarma.Price.ToString("F2");
        }

        /// <summary>
        /// Attempts to validate user input and, if successful, updates the <see cref="Shawarma"/> instance
        /// and closes the dialog with <see langword="true"/>.
        /// </summary>
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

        /// <summary>
        /// Cancels the operation and closes the dialog.
        /// </summary>
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// Validates user input in all fields.
        /// </summary>
        /// <returns><see langword="true"/> if all input is valid; otherwise <see langword="false"/>.</returns>
        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                MessageBox.Show("Please enter a name for the shawarma.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtIngredients.Text))
            {
                MessageBox.Show("Please enter ingredients for the shawarma.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtIngredients.Focus();
                return false;
            }

            if (!decimal.TryParse(TxtPrice.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Please enter a valid price greater than 0.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtPrice.Focus();
                return false;
            }

            return true;
        }
    }
}
