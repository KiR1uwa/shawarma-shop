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
using ShawarmaShop.Infrastructure.Data;

namespace ShawarmaShop
{
    public partial class MenuControl : UserControl
    {
        private AppDbContext dbContext;

        public MenuControl()
        {
            InitializeComponent();
            dbContext = new AppDbContext();
            LoadMenu();

            this.Unloaded += MenuControl_Unloaded;
        }

        private async void LoadMenu()
        {
            try
            {
                var shawarmas = await dbContext.Shawarmas.ToListAsync();
                MenuDataGrid.ItemsSource = shawarmas;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading menu: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddEditShawarmaWindow();
            if (addWindow.ShowDialog() == true)
            {
                try
                {
                    dbContext.Shawarmas.Add(addWindow.Shawarma);
                    await dbContext.SaveChangesAsync();
                    LoadMenu();
                    MessageBox.Show("Shawarma added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding shawarma: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (MenuDataGrid.SelectedItem is Shawarma selectedShawarma)
            {
                var editWindow = new AddEditShawarmaWindow(selectedShawarma);
                if (editWindow.ShowDialog() == true)
                {
                    try
                    {
                        var existingShawarma = await dbContext.Shawarmas.FindAsync(selectedShawarma.Id);
                        if (existingShawarma != null)
                        {
                            existingShawarma.Name = editWindow.Shawarma.Name;
                            existingShawarma.Ingredients = editWindow.Shawarma.Ingredients;
                            existingShawarma.Price = editWindow.Shawarma.Price;
                            await dbContext.SaveChangesAsync();
                            LoadMenu();
                            MessageBox.Show("Shawarma updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating shawarma: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a shawarma to edit.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MenuDataGrid.SelectedItem is Shawarma selectedShawarma)
            {
                var result = MessageBox.Show($"Are you sure you want to delete '{selectedShawarma.Name}'?",
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        dbContext.Shawarmas.Remove(selectedShawarma);
                        await dbContext.SaveChangesAsync();
                        LoadMenu();
                        MessageBox.Show("Shawarma deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting shawarma: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a shawarma to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void MenuControl_Unloaded(object sender, RoutedEventArgs e)
        {
            dbContext?.Dispose();
        }
    }
}