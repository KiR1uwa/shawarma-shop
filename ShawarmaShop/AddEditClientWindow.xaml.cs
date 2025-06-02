using System.Windows;
using ShawarmaShopCore.Models;
using ShawarmaShopCore.Validation;

namespace ShawarmaShop
{
    public partial class AddEditClientWindow : Window
    {
        public Client Client { get; private set; }
        private readonly bool isEditMode;

        public AddEditClientWindow()
        {
            InitializeComponent();
            Client = new Client();
            isEditMode = false;
            Title = "Add New Client";
        }

        public AddEditClientWindow(Client existingClient)
        {
            InitializeComponent();
            Client = new Client
            {
                Id = existingClient.Id,
                Name = existingClient.Name,
                Phone = existingClient.Phone,
                Email = existingClient.Email
            };
            isEditMode = true;
            Title = "Edit Client";

            TxtName.Text = Client.Name;
            TxtPhone.Text = Client.Phone;
            TxtEmail.Text = Client.Email;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string name = TxtName.Text.Trim();
            string phone = TxtPhone.Text.Trim();
            string email = TxtEmail.Text.Trim();

            if (!ClientValidator.IsValidName(name))
            {
                MessageBox.Show("Name must be at least 2 characters long.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtName.Focus();
                return;
            }

            if (!ClientValidator.IsValidPhone(phone))
            {
                MessageBox.Show("Phone must contain at least 10 digits.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtPhone.Focus();
                return;
            }

            if (!ClientValidator.IsValidEmail(email))
            {
                MessageBox.Show("Invalid email format.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtEmail.Focus();
                return;
            }

            Client.Name = name;
            Client.Phone = phone;
            Client.Email = email;

            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
