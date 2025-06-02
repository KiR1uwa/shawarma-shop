using System.Windows;
using System.Windows.Controls;
using ShawarmaShopCore.Interfaces;
using ShawarmaShopCore.Models;
using ShawarmaShopCore.Services;

namespace ShawarmaShop
{
    public partial class ClientsControl : UserControl
    {
        private readonly IClientService _clientService;

        public ClientsControl(IClientService clientService)
        {
            InitializeComponent();
            _clientService = clientService;
            LoadClients();
        }

        private void LoadClients()
        {
            ClientsDataGrid.ItemsSource = _clientService.GetAllClients();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddEditClientWindow();
            if (window.ShowDialog() == true)
            {
                _clientService.AddClient(window.Client);
                LoadClients();
            }
        }

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

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsDataGrid.SelectedItem is Client selectedClient)
            {
                if (MessageBox.Show("Are you sure you want to delete this client?", "Confirm Delete",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _clientService.DeleteClient(selectedClient.Id);
                    LoadClients();
                }
            }
        }
    }
}
