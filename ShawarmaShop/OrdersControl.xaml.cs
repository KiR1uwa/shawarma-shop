using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;

namespace ShawarmaShop
{
    public partial class OrdersControl : UserControl
    {
        private readonly AppDbContext dbContext = new();

        public OrdersControl()
        {
            InitializeComponent();
            LoadOrders();
            Unloaded += OrdersControl_Unloaded;
        }

        private async void LoadOrders()
        {
            var rows = await dbContext.Orders
                .Include(o => o.Client)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Shawarma)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderRow
                {
                    Id = o.Id,
                    ClientName = o.Client.Name,
                    CreatedAt = o.CreatedAt,
                    DeliveryAt = o.DeliveryAt,
                    ItemsSummary = string.Join(", ",
                        o.Items.Select(i => $"{i.Shawarma.Name} x{i.Quantity}")),
                    Comment = o.Comment
                })
                .ToListAsync();
            OrdersDataGrid.ItemsSource = rows;
        }

        private void OrdersControl_Unloaded(object sender, RoutedEventArgs e)
        {
            dbContext.Dispose();
        }

        private sealed record OrderRow
        {
            public int Id { get; init; }
            public string ClientName { get; init; } = "";
            public DateTime CreatedAt { get; init; }
            public DateTime DeliveryAt { get; init; }
            public string ItemsSummary { get; init; } = "";
            public string? Comment { get; init; }
        }
    }
}
