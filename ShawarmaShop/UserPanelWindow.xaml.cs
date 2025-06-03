// =============================================================
// File: UserPanelWindow.xaml.cs
// =============================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Authentication;
using ShawarmaShopCore.Models;
using ShawarmaShop.Infrastructure.Data;

namespace ShawarmaShop
{
    /// <summary>
    ///     Interaction logic for <c>UserPanelWindow</c>.
    ///     Provides an ordering interface for regular (non‑admin) users where they can:
    ///     <list type="bullet">
    ///         <item><description>Add one or more <see cref="Shawarma"/> items to a cart.</description></item>
    ///         <item><description>Select a preferred delivery time (15‑minute granularity).</description></item>
    ///         <item><description>Place the order, which creates the necessary <see cref="Order"/> &amp; <see cref="OrderItem"/> records.</description></item>
    ///     </list>
    ///     The window also supports logging out and returning to the <see cref="LoginWindow"/>.
    /// </summary>
    public partial class UserPanelWindow : Window
    {
        #region fields

        /// <summary>
        ///     EF Core database context. Disposed when the window closes.
        /// </summary>
        private readonly AppDbContext dbContext = new();

        /// <summary>
        ///     Cached list of all available dishes on the menu.
        /// </summary>
        private readonly List<Shawarma> menu;

        /// <summary>
        ///     Reference to the authenticated user that launched this window.
        /// </summary>
        private readonly User currentUser;

        #endregion

        #region ctor

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserPanelWindow"/> class for the specified <paramref name="currentUser"/>.
        ///     Loads the full shawarma menu from the database, builds the delivery‑time combo‑box and sets the initial total.
        /// </summary>
        /// <param name="currentUser">Currently authenticated user (non‑admin).</param>
        public UserPanelWindow(User currentUser)
        {
            InitializeComponent();
            this.currentUser = currentUser;

            // Pre‑load menu so we don't query the DB on every cart change
            menu = dbContext.Shawarmas.ToList();

            FillTimeCombo();
            UpdateTotal();
        }

        #endregion

        #region private‑helpers

        /// <summary>
        ///     Populates the delivery‑time combo‑box (<c>cmbOrderTime</c>) with 15‑minute slots
        ///     starting 30 minutes from <see cref="DateTime.Now"/> and spanning the next 12 hours (48 slots).
        /// </summary>
        private void FillTimeCombo()
        {
            DateTime start = DateTime.Now.AddMinutes(30);
            start = start.AddMinutes(15 - (start.Minute % 15)); // round up to nearest quarter‑hour

            for (int i = 0; i < 48; i++)
            {
                cmbOrderTime.Items.Add(start.ToString("HH:mm"));
                start = start.AddMinutes(15);
            }

            cmbOrderTime.SelectedIndex = 0;
        }

        /// <summary>
        ///     Adds a new <c>CartItemControl</c> to the cart panel and wires up callbacks to
        ///     keep the running total accurate.
        /// </summary>
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var item = new CartItemControl(menu);

            // Recalculate total whenever quantity or selected dish changes
            item.TotalsChanged += (_, _) => UpdateTotal();

            // Support removing the control from the UI
            item.DeleteRequested += (_, _) =>
            {
                CartPanel.Children.Remove(item);
                UpdateTotal();
            };

            // Insert the control just above the "+ Add" button
            int index = CartPanel.Children.IndexOf(btnAdd);
            CartPanel.Children.Insert(index, item);
            UpdateTotal();
        }

        /// <summary>
        ///     Computes the total cart value and shows it in <c>txtTotal</c>.
        /// </summary>
        private void UpdateTotal()
        {
            decimal total = CartPanel.Children
                                     .OfType<CartItemControl>()
                                     .Sum(i => i.UnitPrice * i.Quantity);

            txtTotal.Text = $"Total: {total:F2} zł";
        }

        /// <summary>
        ///     Persists the order and associated <see cref="OrderItem"/> rows after performing validation.
        /// </summary>
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Gather valid cart lines ------------------------------------------------
            var items = CartPanel.Children.OfType<CartItemControl>()
                                          .Where(i => i.SelectedDish != null && i.Quantity > 0)
                                          .ToList();

            if (!items.Any())
            {
                MessageBox.Show("Cart is empty");
                return;
            }

            if (cmbOrderTime.SelectedItem is null)
            {
                MessageBox.Show("Select time");
                return;
            }

            // Ensure client record exists -----------------------------------------
            var client = dbContext.Clients.FirstOrDefault(c => c.Email == currentUser.Email);

            if (client is null)
            {
                client = new Client
                {
                    Name = currentUser.Username,
                    Phone = currentUser.Phone,
                    Email = currentUser.Email
                };

                dbContext.Clients.Add(client);
                dbContext.SaveChanges();
            }

            // Calculate chosen delivery DateTime -----------------------------------
            TimeSpan hhmm = TimeSpan.Parse((string)cmbOrderTime.SelectedItem);
            DateTime delivery = DateTime.Today + hhmm;

            // If the slot is in the past, move to tomorrow
            if (delivery < DateTime.Now.AddMinutes(30))
            {
                delivery = delivery.AddDays(1);
            }

            // Create order header ---------------------------------------------------
            var order = new Order
            {
                CreatedAt = DateTime.Now,
                DeliveryAt = delivery,
                Comment = null,
                ClientID = client.Id
            };

            dbContext.Orders.Add(order);
            dbContext.SaveChanges(); // so we have Order.Id

            // Create order items ----------------------------------------------------
            foreach (var it in items)
            {
                dbContext.OrderItems.Add(new OrderItem
                {
                    OrderId = order.Id,
                    ShawarmaId = it.SelectedDish!.Id,
                    Quantity = it.Quantity
                });
            }

            dbContext.SaveChanges();

            MessageBox.Show("Order saved");

            // Reset UI --------------------------------------------------------------
            CartPanel.Children.Clear();
            CartPanel.Children.Add(btnAdd);
            UpdateTotal();
        }

        /// <summary>
        ///     Logs the current user out by showing the <see cref="LoginWindow"/> again.
        ///     Once a new user logs in, the correct dashboard (admin or user) is opened.
        /// </summary>
        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            var login = new LoginWindow { Owner = this };
            Hide();

            bool? result = login.ShowDialog();

            if (result == true && login.LoggedInUser is not null)
            {
                var user = login.LoggedInUser;

                Window next = user.Role == "admin"
                             ? new MainWindow()
                             : new UserPanelWindow(user);

                Application.Current.MainWindow = next;
                Close();
                next.Show();
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        /// <summary>
        ///     Disposes the EF Core context when the window is closed.
        /// </summary>
        /// <param name="e">Event args supplied by WPF.</param>
        protected override void OnClosed(EventArgs e)
        {
            dbContext.Dispose();
            base.OnClosed(e);
        }

        #endregion
    }
}
