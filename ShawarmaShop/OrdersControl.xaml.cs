using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using ShawarmaShopCore.Models;
using ShawarmaShop.Infrastructure.Data;

namespace ShawarmaShop
{
    /// <summary>
    ///     Interaction logic for <c>OrdersControl</c>.
    ///     Provides a grid view for listing, creating, editing, and deleting orders.
    ///     Data is fetched from the underlying <see cref="AppDbContext"/> and projected
    ///     into a lightweight <see cref="OrderRow"/> for display.
    /// </summary>
    public partial class OrdersControl : UserControl
    {
        private readonly AppDbContext dbContext = new();

        /// <summary>
        ///     Initialises a new instance of the <see cref="OrdersControl"/> class and
        ///     immediately populates the data grid with the most recent orders.
        /// </summary>
        public OrdersControl()
        {
            InitializeComponent();
            LoadOrders();
        }

        /// <summary>
        ///     Retrieves orders from the database, eagerly loads related client and item
        ///     navigation properties, and binds a projected <see cref="OrderRow"/> list
        ///     to <c>OrdersDataGrid</c>.
        /// </summary>
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

        /// <summary>
        ///     Opens an <see cref="OrderWindow"/> in *add* mode.  When the modal dialog closes
        ///     with <c>true</c>, the order list is refreshed.
        /// </summary>
        /// <param name="sender">Button that raised the event.</param>
        /// <param name="e">Routed event data.</param>
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var win = new OrderWindow();
            if (win.ShowDialog() == true) LoadOrders();
        }

        /// <summary>
        ///     Edits the currently selected order.  Displays a warning if no row is
        ///     selected or the order cannot be found in the database.
        /// </summary>
        /// <param name="sender">Button that raised the event.</param>
        /// <param name="e">Routed event data.</param>
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem is not OrderRow row)
            {
                MessageBox.Show("Select order first");
                return;
            }

            var order = dbContext.Orders
                                 .Include(o => o.Items)
                                 .FirstOrDefault(o => o.Id == row.Id);

            if (order is null)
            {
                MessageBox.Show("Order not found");
                return;
            }

            var win = new OrderWindow(order);
            if (win.ShowDialog() == true) LoadOrders();
        }

        /// <summary>
        ///     Deletes the selected order after user confirmation.  Associated
        ///     <see cref="OrderItem"/> entities are removed first to satisfy the FK constraint.
        /// </summary>
        /// <param name="sender">Button that raised the event.</param>
        /// <param name="e">Routed event data.</param>
        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem is not OrderRow row)
            {
                MessageBox.Show("Select order first");
                return;
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

        /// <summary>
        ///     Lightweight projection used solely for display purposes within
        ///     <see cref="OrdersDataGrid"/>.  Keeps the UI decoupled from the full
        ///     <see cref="Order"/> domain entity.
        /// </summary>
        private sealed record OrderRow
        {
            /// <summary>Gets the primary key of the order.</summary>
            public int Id { get; init; }

            /// <summary>Gets the customer’s display name.</summary>
            public string ClientName { get; init; } = string.Empty;

            /// <summary>Gets the timestamp when the order was placed.</summary>
            public DateTime Placed { get; init; }

            /// <summary>Gets the scheduled delivery time.</summary>
            public DateTime Ready { get; init; }

            /// <summary>Gets a concatenated summary like "Classic x2, Spicy x1".</summary>
            public string ItemsSummary { get; init; } = string.Empty;

            /// <summary>Gets the optional comment left by the client.</summary>
            public string? Comment { get; init; }
        }
    }
}
