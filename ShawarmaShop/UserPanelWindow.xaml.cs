using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Authentication;
using ShawarmaShopCore.Models;
using ShawarmaShop.Infrastructure.Data;

namespace ShawarmaShop
{
    public partial class UserPanelWindow : Window
    {
        private readonly AppDbContext dbContext = new();
        private readonly List<Shawarma> menu;
        private readonly User currentUser;

        public UserPanelWindow(User currentUser)
        {
            InitializeComponent();
            this.currentUser = currentUser;
            menu = dbContext.Shawarmas.ToList();
            FillTimeCombo();
            UpdateTotal();
        }

        private void FillTimeCombo()
        {
            DateTime start = DateTime.Now.AddMinutes(30);
            start = start.AddMinutes(15 - (start.Minute % 15));
            for (int i = 0; i < 48; i++)
            {
                cmbOrderTime.Items.Add(start.ToString("HH:mm"));
                start = start.AddMinutes(15);
            }
            cmbOrderTime.SelectedIndex = 0;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var item = new CartItemControl(menu);
            item.TotalsChanged += (_, _) => UpdateTotal();
            item.DeleteRequested += (_, _) =>
            {
                CartPanel.Children.Remove(item);
                UpdateTotal();
            };
            int index = CartPanel.Children.IndexOf(btnAdd);
            CartPanel.Children.Insert(index, item);
            UpdateTotal();
        }

        private void UpdateTotal()
        {
            decimal total = CartPanel.Children
                                     .OfType<CartItemControl>()
                                     .Sum(i => i.UnitPrice * i.Quantity);
            txtTotal.Text = $"Total: {total:F2} zł";
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var items = CartPanel.Children.OfType<CartItemControl>()
                                          .Where(i => i.SelectedDish != null && i.Quantity > 0)
                                          .ToList();
            if (!items.Any()) { MessageBox.Show("Cart is empty"); return; }
            if (cmbOrderTime.SelectedItem is null) { MessageBox.Show("Select time"); return; }

            var client = dbContext.Clients.FirstOrDefault(c => c.Email == currentUser.Email);
            if (client is null)
            {
                client = new Client { Name = currentUser.Username, Phone = currentUser.Phone, Email = currentUser.Email };
                dbContext.Clients.Add(client);
                dbContext.SaveChanges();
            }

            TimeSpan hhmm = TimeSpan.Parse((string)cmbOrderTime.SelectedItem);
            DateTime delivery = DateTime.Today + hhmm;
            if (delivery < DateTime.Now.AddMinutes(30)) delivery = delivery.AddDays(1);

            var order = new Order
            {
                CreatedAt = DateTime.Now,
                DeliveryAt = delivery,
                Comment = null,
                ClientID = client.Id
            };
            dbContext.Orders.Add(order);
            dbContext.SaveChanges();

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
            CartPanel.Children.Clear();
            CartPanel.Children.Add(btnAdd);
            UpdateTotal();
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            var login = new LoginWindow { Owner = this };
            Hide();
            bool? result = login.ShowDialog();

            if (result == true && login.LoggedInUser is not null)
            {
                var user = login.LoggedInUser;
                Window next = user.Role == "admin" ? new MainWindow() : new UserPanelWindow(user);
                Application.Current.MainWindow = next;
                Close();
                next.Show();
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            dbContext.Dispose();
            base.OnClosed(e);
        }
    }
}
