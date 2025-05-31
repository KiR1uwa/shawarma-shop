using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using Client_.Models;
using Order_.Models;
using OrderItem_.Models;

namespace ShawarmaShop;

public partial class OrdersControl : UserControl
{
    private readonly AppDbContext dbContext = new();

    public OrdersControl()
    {
        InitializeComponent();
        LoadOrders();
        Unloaded += (_, _) => dbContext.Dispose();
    }

    private async void LoadOrders()
    {
        var rows = await dbContext.Orders
            .Include(o => o.Client)
            .Select(o => new OrderRow
            {
                Id = o.Id,
                ClientName = o.Client.Name,
                OrderDate = o.DateTime,
                Comment = o.Comment
            })
            .ToListAsync();

        OrdersDataGrid.ItemsSource = rows;
    }

    private void BtnAdd_Click(object sender, RoutedEventArgs e)
    {
        var window = new OrderWindow();
        if (window.ShowDialog() == true)
            LoadOrders();
    }

    private void BtnEdit_Click(object sender, RoutedEventArgs e)
    {
        if (OrdersDataGrid.SelectedItem is not OrderRow row)
        {
            MessageBox.Show("Please select an order to edit.", "No selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var order = dbContext.Orders
            .Include(o => o.Items)
            .FirstOrDefault(o => o.Id == row.Id);

        if (order is null)
        {
            MessageBox.Show("Selected order not found in database.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var window = new OrderWindow(order);
        if (window.ShowDialog() == true)
            LoadOrders();
    }

    private async void BtnDelete_Click(object sender, RoutedEventArgs e)
    {
        if (OrdersDataGrid.SelectedItem is not OrderRow row)
        {
            MessageBox.Show("Please select an order to delete.", "No selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var result = MessageBox.Show($"Are you sure you want to delete order #{row.Id}?",
            "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes) return;

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
        public DateTime OrderDate { get; init; }
        public string? Comment { get; init; }
    }
}
