using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using ShawarmaShopCore.Models;
using ShawarmaShop.Infrastructure.Data;

namespace ShawarmaShop
{
    /// <summary>
    ///     CRUD UI for <see cref="Shawarma"/> items that appear on the menu.
    /// </summary>
    public partial class MenuControl : UserControl
    {
        private readonly AppDbContext dbContext;

        /// <summary>Initialises the control and loads the menu from the database.</summary>
        public MenuControl()
        {
            InitializeComponent();
            dbContext = new AppDbContext();
            LoadMenu();

            Unloaded += MenuControl_Unloaded;
        }

        /// <summary>Fetches all shawarmas from the DB and binds them to <c>MenuDataGrid</c>.</summary>
        private async void LoadMenu()
        {
            try
            {
                MenuDataGrid.ItemsSource = await dbContext.Shawarmas.ToListAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading menu: {ex.Message}",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        // Button handlers ------------------------------------------------------

        /// <summary>Opens the <c>AddEditShawarmaWindow</c> in <em>add</em> mode.</summary>
        private async void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var win = new AddEditShawarmaWindow();
            if (win.ShowDialog() != true) return;

            try
            {
                dbContext.Shawarmas.Add(win.Shawarma);
                await dbContext.SaveChangesAsync();
                LoadMenu();
                MessageBox.Show("Shawarma added successfully!",
                                "Success",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding shawarma: {ex.Message}",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        /// <summary>Edits the selected <see cref="Shawarma"/> record.</summary>
        private async void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (MenuDataGrid.SelectedItem is not Shawarma selected)
            {
                MessageBox.Show("Please select a shawarma to edit.",
                                "No Selection",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            var win = new AddEditShawarmaWindow(selected);
            if (win.ShowDialog() != true) return;

            try
            {
                var entity = await dbContext.Shawarmas.FindAsync(selected.Id);
                if (entity != null)
                {
                    entity.Name = win.Shawarma.Name;
                    entity.Ingredients = win.Shawarma.Ingredients;
                    entity.Price = win.Shawarma.Price;

                    await dbContext.SaveChangesAsync();
                    LoadMenu();
                    MessageBox.Show("Shawarma updated successfully!",
                                    "Success",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating shawarma: {ex.Message}",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        /// <summary>Deletes the selected <see cref="Shawarma"/> from the database.</summary>
        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MenuDataGrid.SelectedItem is not Shawarma selected)
            {
                MessageBox.Show("Please select a shawarma to delete.",
                                "No Selection",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                $"Are you sure you want to delete '{selected.Name}'?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                dbContext.Shawarmas.Remove(selected);
                await dbContext.SaveChangesAsync();
                LoadMenu();
                MessageBox.Show("Shawarma deleted successfully!",
                                "Success",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting shawarma: {ex.Message}",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        /// <summary>Disposes the EF Core context when the control is unloaded.</summary>
        private void MenuControl_Unloaded(object? sender, RoutedEventArgs e) => dbContext.Dispose();
    }
}
