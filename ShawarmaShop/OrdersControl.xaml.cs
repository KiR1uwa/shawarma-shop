using System;
using System.Linq;
using System.Collections.Generic;
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

    private void BtnAdd_Click(object sender, RoutedEventArgs e) => OpenOrderWindow(null);

    private void BtnEdit_Click(object sender, RoutedEventArgs e)
    {
        if (OrdersDataGrid.SelectedItem is not OrderRow row) return;
        var order = dbContext.Orders
            .Include(o => o.Items)
            .First(o => o.Id == row.Id);
        OpenOrderWindow(order);
    }

    private async void BtnDelete_Click(object sender, RoutedEventArgs e)
    {
        if (OrdersDataGrid.SelectedItem is not OrderRow row) return;

        if (MessageBox.Show($"Delete order #{row.Id}?",
            "Confirm", MessageBoxButton.YesNo,
            MessageBoxImage.Question) != MessageBoxResult.Yes) return;

        var order = await dbContext.Orders.FindAsync(row.Id);
        dbContext.Orders.Remove(order);
        await dbContext.SaveChangesAsync();
        LoadOrders();
    }

    private void OpenOrderWindow(Order? order)
    {
        var wnd = order is null ? new OrderWindow() : new OrderWindow(order);
        if (wnd.ShowDialog() == true) LoadOrders();
    }

    private sealed record OrderRow
    {
        public int Id { get; init; }
        public string ClientName { get; init; } = "";
        public DateTime OrderDate { get; init; }
        public string? Comment { get; init; }
    }
}
