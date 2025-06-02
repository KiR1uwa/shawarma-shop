using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using ShawarmaShopCore.Models;

namespace ShawarmaShop
{
    public partial class OrdersControl : UserControl
    {
        private readonly AppDbContext dbContext = new();

        public OrdersControl()
        {
            InitializeComponent();
            LoadOrders();
        }

        private async void LoadOrders()
        {
            OrdersDataGrid.ItemsSource = await dbContext.Orders
                .Include(o => o.Client)
                .Include(o => o.Items).ThenInclude(i => i.Shawarma)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderRow
                {
                    Id = o.Id,
                    ClientName = o.Client.Name,
                    Placed = o.CreatedAt,
                    Ready = o.DeliveryAt,
                    ItemsSummary = string.Join(", ", o.Items.Select(i => $"{i.Shawarma.Name} x{i.Quantity}")),
                    Comment = o.Comment
                })
                .ToListAsync();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var win = new OrderWindow();
            if (win.ShowDialog() == true) LoadOrders();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem is not OrderRow row)
            {
                MessageBox.Show("Select order first"); return;
            }

            var order = dbContext.Orders
                .Include(o => o.Items)
                .FirstOrDefault(o => o.Id == row.Id);

            if (order is null)
            {
                MessageBox.Show("Order not found"); return;
            }

            var win = new OrderWindow(order);
            if (win.ShowDialog() == true) LoadOrders();
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem is not OrderRow row)
            {
                MessageBox.Show("Select order first"); return;
            }

            if (MessageBox.Show($"Delete order #{row.Id}?", "Confirm", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            var order = await dbContext.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == row.Id);

            if (order is not null)
            {
                dbContext.OrderItems.RemoveRange(order.Items);
                dbContext.Orders.Remove(order);
                await dbContext.SaveChangesAsync();
                LoadOrders();
            }
        }

        private sealed record OrderRow
        {
            public int Id { get; init; }
            public string ClientName { get; init; } = "";
            public DateTime Placed { get; init; }
            public DateTime Ready { get; init; }
            public string ItemsSummary { get; init; } = "";
            public string? Comment { get; init; }
        }
    }
}
