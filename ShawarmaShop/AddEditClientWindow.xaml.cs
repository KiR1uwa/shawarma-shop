// =====================================
// File: AddEditClientWindow.xaml.cs
// =====================================

using System.Windows;
using ShawarmaShopCore.Models;
using ShawarmaShopCore.Validation;

namespace ShawarmaShop
{
    /// <summary>
    /// Interaction logic for <c>AddEditClientWindow</c>.
    /// Provides UI for creating a new <see cref="Client"/> or editing an existing one.
    /// </summary>
    public partial class AddEditClientWindow : Window
    {
        /// <summary>
        /// Gets the <see cref="Client"/> instance being created or edited by the dialog.
        /// </summary>
        public Client Client { get; private set; }

        /// <summary>
        /// Indicates whether the window is opened in edit mode (<see langword="true"/>)
        /// or in add‑new mode (<see langword="false"/>).
        /// </summary>
        private readonly bool isEditMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddEditClientWindow"/> class
        /// in <em>add‑new</em> mode.
        /// </summary>
        public AddEditClientWindow()
        {
            InitializeComponent();
            Client = new Client();
            isEditMode = false;
            Title = "Add New Client";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddEditClientWindow"/> class
        /// in <em>edit</em> mode and pre‑loads UI controls with data from the supplied client.
        /// </summary>
        /// <param name="existingClient">Client to edit. Must not be <see langword="null"/>.</param>
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

            // Pre‑populate input fields with existing data.
            TxtName.Text = Client.Name;
            TxtPhone.Text = Client.Phone;
            TxtEmail.Text = Client.Email;
        }

        /// <summary>
        /// Validates user input, updates <see cref="Client"/> properties and closes the dialog
        /// with a positive <see cref="Window.DialogResult"/> when validation succeeds.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event data.</param>
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

        /// <summary>
        /// Cancels the operation and closes the dialog, setting
        /// <see cref="Window.DialogResult"/> to <see langword="false"/>.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event data.</param>
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}