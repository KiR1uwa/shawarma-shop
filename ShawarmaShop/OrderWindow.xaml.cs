using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Client_.Models;
using Microsoft.EntityFrameworkCore;
using Order_.Models;
using OrderItem_.Models;
using Shawarma_.Models;

namespace ShawarmaShop
{
    public partial class OrderWindow : Window, INotifyPropertyChanged
    {
        private readonly AppDbContext dbContext = new();
        private readonly bool isEditMode;
        private readonly Order order;
        private readonly ObservableCollection<ShawarmaItemViewModel> shawarmaItems = new();

        public OrderWindow()
        {
            InitializeComponent();
            order = new Order();
            isEditMode = false;
            Title = "Create order";
            InitData();
        }

        public OrderWindow(Order existingOrder)
        {
            InitializeComponent();
            order = dbContext.Orders.Include(o => o.Items).First(o => o.Id == existingOrder.Id);
            isEditMode = true;
            Title = "Edit order";
            InitData();
            LoadOrder();
        }

        private async void InitData()
        {
            CmbClients.ItemsSource = await dbContext.Clients.ToListAsync();
            var list = await dbContext.Shawarmas.ToListAsync();
            foreach (var s in list)
            {
                var vm = new ShawarmaItemViewModel(s);
                vm.PropertyChanged += (_, _) => RecalcTotal();
                shawarmaItems.Add(vm);
            }
            ShawarmaItemsGrid.ItemsSource = shawarmaItems;
            DpDeliveryDate.SelectedDate = DateTime.Today;
            TxtDeliveryTime.Text = DateTime.Now.ToString("HH:mm");
            RecalcTotal();
            CmbClients_SelectionChanged(null, null);
        }

        private void LoadOrder()
        {
            CmbClients.SelectedValue = order.ClientID;
            DpDeliveryDate.SelectedDate = order.DeliveryAt.Date;
            TxtDeliveryTime.Text = order.DeliveryAt.ToString("HH:mm");
            TxtComment.Text = order.Comment;
            foreach (var it in order.Items)
            {
                var vm = shawarmaItems.First(x => x.Id == it.ShawarmaId);
                vm.IsSelected = true;
                vm.Quantity = it.Quantity;
            }
            RecalcTotal();
        }

        private void CmbClients_SelectionChanged(object? sender, System.Windows.Controls.SelectionChangedEventArgs? e)
        {
            if (CmbClients.SelectedItem is Client c)
            {
                TxtClientPhone.Text = c.Phone;
                TxtClientEmail.Text = c.Email;
            }
            else
            {
                TxtClientPhone.Text = "";
                TxtClientEmail.Text = "";
            }
        }

        private void RecalcTotal()
        {
            decimal total = shawarmaItems.Where(x => x.IsSelected && x.Quantity > 0).Sum(x => x.Subtotal);
            TxtTotal.Text = total.ToString("F2");
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!Validate()) return;

            var time = TimeSpan.ParseExact(TxtDeliveryTime.Text, @"hh\:mm", CultureInfo.InvariantCulture);
            DateTime delivery = DpDeliveryDate.SelectedDate!.Value.Date + time;

            if (!isEditMode) order.CreatedAt = DateTime.Now;
            order.DeliveryAt = delivery;
            order.ClientID = ((Client)CmbClients.SelectedItem!).Id;
            order.Comment = string.IsNullOrWhiteSpace(TxtComment.Text) ? null : TxtComment.Text.Trim();

            dbContext.OrderItems.RemoveRange(order.Items);
            order.Items.Clear();

            foreach (var vm in shawarmaItems.Where(x => x.IsSelected && x.Quantity > 0))
            {
                order.Items.Add(new OrderItem { ShawarmaId = vm.Id, Quantity = vm.Quantity });
            }

            if (isEditMode)
                dbContext.Orders.Update(order);
            else
                dbContext.Orders.Add(order);

            dbContext.SaveChanges();
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => Close();

        private bool Validate()
        {
            if (CmbClients.SelectedItem == null) { MessageBox.Show("Select client"); return false; }
            if (DpDeliveryDate.SelectedDate == null) { MessageBox.Show("Select date"); return false; }
            if (!TimeSpan.TryParseExact(TxtDeliveryTime.Text, @"hh\:mm", CultureInfo.InvariantCulture, out _)) { MessageBox.Show("Time format HH:mm"); return false; }
            if (!shawarmaItems.Any(x => x.IsSelected && x.Quantity > 0)) { MessageBox.Show("Select at least one item"); return false; }
            return true;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string p = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));

        public sealed class ShawarmaItemViewModel : INotifyPropertyChanged
        {
            public int Id { get; }
            public string Name { get; }
            public decimal Price { get; }
            private bool isSelected;
            private int quantity = 1;

            public ShawarmaItemViewModel(Shawarma s) { Id = s.Id; Name = s.Name; Price = s.Price; }

            public bool IsSelected { get => isSelected; set { isSelected = value; OnChanged(); } }
            public int Quantity { get => quantity; set { if (value > 0) { quantity = value; OnChanged(); } } }
            public decimal Subtotal => IsSelected ? Price * Quantity : 0;

            public event PropertyChangedEventHandler? PropertyChanged;
            private void OnChanged([CallerMemberName] string p = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }
    }
}
