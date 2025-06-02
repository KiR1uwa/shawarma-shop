using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;
using ShawarmaShopCore.Models;

namespace ShawarmaShop
{
    public partial class ClientsControl : UserControl
    {
        private AppDbContext dbContext;

        public ClientsControl()
        {
            InitializeComponent();
            dbContext = new AppDbContext();
            LoadClients();
            this.Unloaded += ClientsControl_Unloaded;
        }

        private async void LoadClients()
        {
            try
            {
                var clients = await dbContext.Clients.ToListAsync();
                ClientsDataGrid.ItemsSource = clients;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading clients: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddEditClientWindow();
            if (addWindow.ShowDialog() == true)
            {
                try
                {
                    dbContext.Clients.Add(addWindow.Client);
                    await dbContext.SaveChangesAsync();
                    LoadClients();
                    MessageBox.Show("Client added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding client: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsDataGrid.SelectedItem is Client selectedClient)
            {
                var editWindow = new AddEditClientWindow(selectedClient);
                if (editWindow.ShowDialog() == true)
                {
                    try
                    {
                        var existingClient = await dbContext.Clients.FindAsync(selectedClient.Id);
                        if (existingClient != null)
                        {
                            existingClient.Name = editWindow.Client.Name;
                            existingClient.Phone = editWindow.Client.Phone;
                            existingClient.Email = editWindow.Client.Email;
                            await dbContext.SaveChangesAsync();
                            LoadClients();
                            MessageBox.Show("Client updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating client: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a client to edit.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsDataGrid.SelectedItem is Client selectedClient)
            {
                var result = MessageBox.Show($"Are you sure you want to delete client '{selectedClient.Name}'?",
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        dbContext.Clients.Remove(selectedClient);
                        await dbContext.SaveChangesAsync();
                        LoadClients();
                        MessageBox.Show("Client deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting client: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a client to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ClientsControl_Unloaded(object sender, RoutedEventArgs e)
        {
            dbContext?.Dispose();
        }
    }
}