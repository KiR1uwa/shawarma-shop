using System;
using System.Text.RegularExpressions;
using System.Windows;
using ShawarmaShopCore.Models;

namespace ShawarmaShop
{
    public partial class AddEditClientWindow : Window
    {
        public Client Client { get; private set; }
        private bool isEditMode;

        public AddEditClientWindow()
        {
            InitializeComponent();
            isEditMode = false;
            Client = new Client();
            Title = "Add New Client";
        }

        public AddEditClientWindow(Client client)
        {
            InitializeComponent();
            isEditMode = true;
            Client = new Client
            {
                Id = client.Id,
                Name = client.Name,
                Phone = client.Phone,
                Email = client.Email
            };
            Title = "Edit Client";
            LoadData();
        }

        private void LoadData()
        {
            TxtName.Text = Client.Name;
            TxtPhone.Text = Client.Phone;
            TxtEmail.Text = Client.Email;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput())
            {
                Client.Name = TxtName.Text.Trim();
                Client.Phone = TxtPhone.Text.Trim();
                Client.Email = TxtEmail.Text.Trim();

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
                MessageBox.Show("Please enter a name for the client.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtPhone.Text))
            {
                MessageBox.Show("Please enter a phone number.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtPhone.Focus();
                return false;
            }

            if (!IsValidPhoneNumber(TxtPhone.Text.Trim()))
            {
                MessageBox.Show("Please enter a valid phone number (e.g., +48123456789).", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtPhone.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtEmail.Text))
            {
                MessageBox.Show("Please enter an email address.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtEmail.Focus();
                return false;
            }

            if (!IsValidEmail(TxtEmail.Text.Trim()))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtEmail.Focus();
                return false;
            }

            return true;
        }

        private bool IsValidPhoneNumber(string phone)
        {
            var phoneRegex = new Regex(@"^\+?[1-9]\d{8,14}$");
            return phoneRegex.IsMatch(phone);
        }

        private bool IsValidEmail(string email)
        {
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return emailRegex.IsMatch(email);
        }
    }
}