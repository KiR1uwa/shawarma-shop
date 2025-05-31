using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Order_.Models;
using Client_.Models;
using Shawarma_.Models;
using OrderItem_.Models;

namespace ShawarmaShop
{
    public partial class OrderWindow : Window
    {
        private AppDbContext dbContext;
        private Order order;
        private bool isEditMode;
        private ObservableCollection<ShawarmaItemViewModel> shawarmaItems;

        public OrderWindow()
        {
            InitializeComponent();
            dbContext = new AppDbContext();
            order = new Order();
            isEditMode = false;
            Title = "Create New Order";
            shawarmaItems = new ObservableCollection<ShawarmaItemViewModel>();
            InitializeData();
        }

        public OrderWindow(Order existingOrder)
        {
            InitializeComponent();
            dbContext = new AppDbContext();
            order = existingOrder;
            isEditMode = true;
            Title = "Edit Order";
            shawarmaItems = new ObservableCollection<ShawarmaItemViewModel>();
            InitializeData();
            LoadOrderData();
        }

        private async void InitializeData()
        {
            try
            {
                var clients = await dbContext.Clients.ToListAsync();
                CmbClients.ItemsSource = clients;

                var shawarmas = await dbContext.Shawarmas.ToListAsync();
                foreach (var shawarma in shawarmas)
                {
                    var itemVM = new ShawarmaItemViewModel(shawarma);
                    itemVM.PropertyChanged += ShawarmaItem_PropertyChanged;
                    shawarmaItems.Add(itemVM);
                }

                ShawarmaItemsGrid.ItemsSource = shawarmaItems;
                DpOrderDate.SelectedDate = DateTime.Now.Date;
                TxtTime.Text = DateTime.Now.ToString("HH:mm");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadOrderData()
        {
            if (isEditMode && order != null)
            {
                CmbClients.SelectedValue = order.ClientID;
                DpOrderDate.SelectedDate = order.DateTime.Date;
                TxtTime.Text = order.DateTime.ToString("HH:mm");
                TxtComment.Text = order.Comment ?? "";

                foreach (var orderItem in order.Items)
                {
                    var shawarmaVM = shawarmaItems.FirstOrDefault(s => s.Id == orderItem.ShawarmaId);
                    if (shawarmaVM != null)
                    {
                        shawarmaVM.IsSelected = true;
                        shawarmaVM.Quantity = orderItem.Quantity;
                    }
                }
            }
        }

        private void ShawarmaItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ShawarmaItemViewModel.IsSelected) ||
                e.PropertyName == nameof(ShawarmaItemViewModel.Quantity))
            {
                UpdateTotal();
            }
        }

        private void UpdateTotal()
        {
            var total = shawarmaItems
                .Where(s => s.IsSelected && s.Quantity > 0)
                .Sum(s => s.Subtotal);
            TxtTotal.Text = total.ToString("F2");
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput())
            {
                try
                {
                    if (!DateTime.TryParseExact(TxtTime.Text, "HH:mm", CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out var time))
                    {
                        MessageBox.Show("Invalid time format. Please use HH:MM format.", "Validation Error",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var orderDateTime = DpOrderDate.SelectedDate.Value.Date.Add(time.TimeOfDay);

                    if (isEditMode)
                    {
                        order.ClientID = (int)CmbClients.SelectedValue;
                        order.DateTime = orderDateTime;
                        order.Comment = string.IsNullOrWhiteSpace(TxtComment.Text) ? null : TxtComment.Text.Trim();

                        dbContext.OrderItems.RemoveRange(order.Items);
                        order.Items.Clear();

                        foreach (var item in shawarmaItems.Where(s => s.IsSelected && s.Quantity > 0))
                        {
                            order.Items.Add(new OrderItem
                            {
                                ShawarmaId = item.Id,
                                Quantity = item.Quantity,
                                OrderId = order.Id
                            });
                        }

                        dbContext.Orders.Update(order);
                    }
                    else
                    {
                        order.ClientID = (int)CmbClients.SelectedValue;
                        order.DateTime = orderDateTime;
                        order.Comment = string.IsNullOrWhiteSpace(TxtComment.Text) ? null : TxtComment.Text.Trim();

                        foreach (var item in shawarmaItems.Where(s => s.IsSelected && s.Quantity > 0))
                        {
                            order.Items.Add(new OrderItem
                            {
                                ShawarmaId = item.Id,
                                Quantity = item.Quantity
                            });
                        }

                        dbContext.Orders.Add(order);
                    }

                    await dbContext.SaveChangesAsync();
                    DialogResult = true;
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving order: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool ValidateInput()
        {
            if (CmbClients.SelectedValue == null)
            {
                MessageBox.Show("Please select a client.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                CmbClients.Focus();
                return false;
            }

            if (DpOrderDate.SelectedDate == null)
            {
                MessageBox.Show("Please select a date.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                DpOrderDate.Focus();
                return false;
            }

            if (!DateTime.TryParseExact(TxtTime.Text, "HH:mm", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out _))
            {
                MessageBox.Show("Please enter a valid time in HH:MM format.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtTime.Focus();
                return false;
            }

            if (!shawarmaItems.Any(s => s.IsSelected && s.Quantity > 0))
            {
                MessageBox.Show("Please select at least one shawarma item.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        protected override void OnClosed(EventArgs e)
        {
            dbContext?.Dispose();
            base.OnClosed(e);
        }
    }

    public class ShawarmaItemViewModel : INotifyPropertyChanged
    {
        private bool isSelected;
        private int quantity = 1;

        public int Id { get; set; }
        public string Name { get; set; } = "";
        public decimal Price { get; set; }

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Subtotal));
            }
        }

        public int Quantity
        {
            get => quantity;
            set
            {
                if (value >= 0)
                {
                    quantity = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Subtotal));
                }
            }
        }

        public decimal Subtotal => IsSelected ? Price * Quantity : 0;

        public ShawarmaItemViewModel(Shawarma shawarma)
        {
            Id = shawarma.Id;
            Name = shawarma.Name;
            Price = shawarma.Price;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}