// =====================================
// File: ClientsControl.xaml.cs
// =====================================

using System.Windows;
using System.Windows.Controls;
using ShawarmaShopCore.Interfaces;
using ShawarmaShopCore.Models;

namespace ShawarmaShop
{
    /// <summary>
    /// Interaction logic for <c>ClientsControl</c>.
    /// Presents CRUD operations for <see cref="Client"/> entities.
    /// </summary>
    public partial class ClientsControl : UserControl
    {
        /// <summary>
        /// Abstraction over client‑related data operations.
        /// </summary>
        private readonly IClientService _clientService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientsControl"/> class.
        /// </summary>
        /// <param name="clientService">Concrete implementation of <see cref="IClientService"/>.</param>
        public ClientsControl(IClientService clientService)
        {
            InitializeComponent();
            _clientService = clientService;
            LoadClients();
        }

        /// <summary>
        /// Retrieves all clients from the service and binds them to the <c>DataGrid</c>.
        /// </summary>
        private void LoadClients() =>
            ClientsDataGrid.ItemsSource = _clientService.GetAllClients();

        /// <summary>
        /// Opens the add‑client dialog and persists the new client when the dialog returns <c>true</c>.
        /// </summary>
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddEditClientWindow();
            if (window.ShowDialog() == true)
            {
                _clientService.AddClient(window.Client);
                LoadClients();
            }
        }

        /// <summary>
        /// Opens the edit dialog for the currently selected client and saves changes.
        /// </summary>
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsDataGrid.SelectedItem is Client selectedClient)
            {
                var window = new AddEditClientWindow(selectedClient);
                if (window.ShowDialog() == true)
                {
                    _clientService.UpdateClient(window.Client);
                    LoadClients();
                }
            }
        }

        /// <summary>
        /// Confirms and removes the currently selected client.
        /// </summary>
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsDataGrid.SelectedItem is Client selectedClient)
            {
                if (MessageBox.Show("Are you sure you want to delete this client?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _clientService.DeleteClient(selectedClient.Id);
                    LoadClients();
                }
            }
        }
    }
}