// =============================================================
// File: OrderWindow.xaml.cs
// =============================================================

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using ShawarmaShopCore.Models;
using Microsoft.EntityFrameworkCore;
using ShawarmaShop.Infrastructure.Data;

namespace ShawarmaShop
{
    /// <summary>
    ///     Modal window that allows an operator to create a new <see cref="Order"/>
    ///     or edit an existing one. Implements <see cref="INotifyPropertyChanged"/> so
    ///     that UI elements (e.g. the total amount) stay in sync when the underlying
    ///     data changes.
    /// </summary>
    public partial class OrderWindow : Window, INotifyPropertyChanged
    {
        /// <summary>
        ///     Shared EF Core database context. Its lifetime is scoped to that of the
        ///     window and is disposed implicitly when the process exits, so we keep the
        ///     field <c>readonly</c> and do not implement <see cref="IDisposable"/>.
        /// </summary>
        private readonly AppDbContext dbContext = new();

        /// <summary>
        ///     Indicates whether the window is in <em>edit</em> mode (<see langword="true"/>)
        ///     or <em>create</em> mode (<see langword="false"/>). This affects how the order
        ///     is persisted when the user clicks <c>Save</c>.
        /// </summary>
        private readonly bool isEditMode;

        /// <summary>
        ///     The <see cref="Order"/> entity being edited or created by this window.
        ///     In <em>create</em> mode this is a freshly‑constructed instance; in
        ///     <em>edit</em> mode it is loaded from the database together with its
        ///     navigation properties.
        /// </summary>
        private readonly Order order;

        /// <summary>
        ///     Collection of <see cref="ShawarmaItemViewModel"/>s that wrap the full list
        ///     of available <see cref="Shawarma"/>s. Bound to the grid so the user can
        ///     pick items and quantities. Implements change notification so that totals
        ///     recalculate on the fly.
        /// </summary>
        private readonly ObservableCollection<ShawarmaItemViewModel> shawarmaItems = new();

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of <see cref="OrderWindow"/> for creating a
        ///     brand‑new order.
        /// </summary>
        public OrderWindow()
        {
            InitializeComponent();
            order = new Order();
            isEditMode = false;
            Title = "Create order";
            InitData();
        }

        /// <summary>
        ///     Initializes a new instance of <see cref="OrderWindow"/> for editing the
        ///     specified existing <paramref name="existingOrder"/>.
        /// </summary>
        /// <param name="existingOrder">Order instance to edit. Must already exist in the DB.</param>
        public OrderWindow(Order existingOrder)
        {
            InitializeComponent();
            order = dbContext.Orders.Include(o => o.Items)
                                    .First(o => o.Id == existingOrder.Id);
            isEditMode = true;
            Title = "Edit order";
            InitData();
            LoadOrder();
        }

        #endregion

        #region Data initialisation & loading

        /// <summary>
        ///     Populates combo‑boxes and grids with data from the database, sets sensible
        ///     defaults (today’s date, current time) and pre‑wires event handlers so that
        ///     totals recalculate whenever the user tweaks item selections or quantities.
        /// </summary>
        private async void InitData()
        {
            // Populate clients combo‑box
            CmbClients.ItemsSource = await dbContext.Clients.ToListAsync();

            // Populate shawarma catalogue grid
            var list = await dbContext.Shawarmas.ToListAsync();
            foreach (var s in list)
            {
                var vm = new ShawarmaItemViewModel(s);
                vm.PropertyChanged += (_, _) => RecalcTotal();
                shawarmaItems.Add(vm);
            }
            ShawarmaItemsGrid.ItemsSource = shawarmaItems;

            // Default delivery date/time
            DpDeliveryDate.SelectedDate = DateTime.Today;
            TxtDeliveryTime.Text = DateTime.Now.ToString("HH:mm");

            RecalcTotal();
            CmbClients_SelectionChanged(null, null);
        }

        /// <summary>
        ///     Loads the fields of <see cref="order"/> into the UI so the operator can
        ///     edit them. Only called in <em>edit</em> mode.
        /// </summary>
        private void LoadOrder()
        {
            // Basic order info
            CmbClients.SelectedValue = order.ClientID;
            DpDeliveryDate.SelectedDate = order.DeliveryAt.Date;
            TxtDeliveryTime.Text = order.DeliveryAt.ToString("HH:mm");
            TxtComment.Text = order.Comment;

            // Items and quantities
            foreach (var it in order.Items)
            {
                var vm = shawarmaItems.First(x => x.Id == it.ShawarmaId);
                vm.IsSelected = true;
                vm.Quantity = it.Quantity;
            }
            RecalcTotal();
        }

        #endregion

        #region Event handlers

        /// <summary>
        ///     Updates the read‑only phone and e‑mail text‑blocks when a client is picked
        ///     from the combo‑box.
        /// </summary>
        private void CmbClients_SelectionChanged(object? sender, System.Windows.Controls.SelectionChangedEventArgs? e)
        {
            if (CmbClients.SelectedItem is Client c)
            {
                TxtClientPhone.Text = c.Phone;
                TxtClientEmail.Text = c.Email;
            }
            else
            {
                TxtClientPhone.Text = string.Empty;
                TxtClientEmail.Text = string.Empty;
            }
        }

        /// <summary>
        ///     Recalculates <c>TxtTotal</c> whenever the item list changes.
        /// </summary>
        private void RecalcTotal()
        {
            decimal total = shawarmaItems.Where(x => x.IsSelected && x.Quantity > 0)
                                         .Sum(x => x.Subtotal);
            TxtTotal.Text = total.ToString("F2");
        }

        /// <summary>
        ///     Validates user input, persists the order to the database and closes the
        ///     dialog with <see cref="Window.DialogResult"/> = <c>true</c> if everything is
        ///     OK. Displays message boxes for validation errors.
        /// </summary>
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!Validate()) return;

            // Build DateTime for requested delivery
            var time = TimeSpan.ParseExact(TxtDeliveryTime.Text, @"hh\:mm", CultureInfo.InvariantCulture);
            DateTime delivery = DpDeliveryDate.SelectedDate!.Value.Date + time;

            // Map UI to domain entity
            if (!isEditMode) order.CreatedAt = DateTime.Now;
            order.DeliveryAt = delivery;
            order.ClientID = ((Client)CmbClients.SelectedItem!).Id;
            order.Comment = string.IsNullOrWhiteSpace(TxtComment.Text) ? null : TxtComment.Text.Trim();

            // Replace items collection
            dbContext.OrderItems.RemoveRange(order.Items);
            order.Items.Clear();
            foreach (var vm in shawarmaItems.Where(x => x.IsSelected && x.Quantity > 0))
            {
                order.Items.Add(new OrderItem { ShawarmaId = vm.Id, Quantity = vm.Quantity });
            }

            // Insert vs update
            if (isEditMode)
                dbContext.Orders.Update(order);
            else
                dbContext.Orders.Add(order);

            dbContext.SaveChanges();
            DialogResult = true;
            Close();
        }

        /// <summary>
        ///     Closes the dialog without saving changes.
        /// </summary>
        private void BtnCancel_Click(object sender, RoutedEventArgs e) => Close();

        #endregion

        #region Validation helpers

        /// <summary>
        ///     Performs basic data‑entry validation and shows message boxes for failures.
        /// </summary>
        /// <returns><see langword="true"/> if everything looks good; otherwise <see langword="false"/>.</returns>
        private bool Validate()
        {
            if (CmbClients.SelectedItem == null)
            {
                MessageBox.Show("Select client");
                return false;
            }
            if (DpDeliveryDate.SelectedDate == null)
            {
                MessageBox.Show("Select date");
                return false;
            }
            if (!TimeSpan.TryParseExact(TxtDeliveryTime.Text, @"hh\:mm", CultureInfo.InvariantCulture, out _))
            {
                MessageBox.Show("Time format HH:mm");
                return false;
            }
            if (!shawarmaItems.Any(x => x.IsSelected && x.Quantity > 0))
            {
                MessageBox.Show("Select at least one item");
                return false;
            }
            return true;
        }

        #endregion

        #region INotifyPropertyChanged

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        ///     Raises <see cref="PropertyChanged"/>. Use <c>[CallerMemberName]</c> so the
        ///     caller does not need to pass a property name.
        /// </summary>
        /// <param name="p">Caller‑supplied property name (automatically filled).</param>
        private void OnPropertyChanged([CallerMemberName] string p = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));

        #endregion

        #region Nested view‑model

        /// <summary>
        ///     Lightweight wrapper around <see cref="Shawarma"/> so we can add view‑specific
        ///     state (<see cref="IsSelected"/>, <see cref="Quantity"/>) and raise
        ///     <see cref="INotifyPropertyChanged"/> events for the UI.
        /// </summary>
        public sealed class ShawarmaItemViewModel : INotifyPropertyChanged
        {
            /// <summary>Primary key of the wrapped <see cref="Shawarma"/>.</summary>
            public int Id { get; }
            /// <summary>Name of the shawarma.</summary>
            public string Name { get; }
            /// <summary>Price per single unit.</summary>
            public decimal Price { get; }

            private bool isSelected;
            private int quantity = 1;

            /// <summary>
            ///     Creates a new view‑model from the supplied <paramref name="s"/>.
            /// </summary>
            public ShawarmaItemViewModel(Shawarma s)
            {
                Id = s.Id;
                Name = s.Name;
                Price = s.Price;
            }

            /// <summary>
            ///     Gets or sets whether the item is included in the order.
            /// </summary>
            public bool IsSelected
            {
                get => isSelected;
                set { isSelected = value; OnChanged(); }
            }

            /// <summary>
            ///     Gets or sets the quantity (must be &gt; 0). Setting <c>0</c> or a negative
            ///     value is ignored.
            /// </summary>
            public int Quantity
            {
                get => quantity;
                set { if (value > 0) { quantity = value; OnChanged(); } }
            }

            /// <summary>
            ///     Gets the subtotal (<c>Price × Quantity</c>) if the item is selected;
            ///     otherwise <c>0</c>.
            /// </summary>
            public decimal Subtotal => IsSelected ? Price * Quantity : 0;

            /// <inheritdoc/>
            public event PropertyChangedEventHandler? PropertyChanged;

            /// <summary>Raises <see cref="PropertyChanged"/>.</summary>
            private void OnChanged([CallerMemberName] string p = "") =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }

        #endregion
    }
}
