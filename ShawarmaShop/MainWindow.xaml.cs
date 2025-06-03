using System.Windows;
using ShawarmaShopCore.Services;
using ShawarmaShopCore.Interfaces;
using ShawarmaShop.Infrastructure.Data;

namespace ShawarmaShop
{
    /// <summary>
    ///     The administrator dashboard.  
    ///     Provides navigation between menu, clients and orders modules.
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MenuControl menuControl;
        private readonly ClientsControl clientsControl;
        private readonly OrdersControl ordersControl;

        /// <summary>
        ///     Creates a new <see cref="MainWindow"/> and wires up
        ///     the three content controls.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            var dbContext = new AppDbContext();
            IClientService svc = new ClientService(dbContext);

            menuControl = new MenuControl();
            clientsControl = new ClientsControl(svc);
            ordersControl = new OrdersControl();

            // default view
            MainFrame.Content = menuControl;
        }

        // Navigation buttons ---------------------------------------------------

        private void BtnMenu_Click(object sender, RoutedEventArgs e) => MainFrame.Content = menuControl;
        private void BtnClients_Click(object sender, RoutedEventArgs e) => MainFrame.Content = clientsControl;
        private void BtnOrders_Click(object sender, RoutedEventArgs e) => MainFrame.Content = ordersControl;

        /// <summary>
        ///     Logs the current user out and shows the login screen again.
        ///     When the login completes the correct dashboard is opened
        ///     based on the user’s role.
        /// </summary>
        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            var login = new LoginWindow { Owner = this };
            Hide();

            bool? ok = login.ShowDialog();
            if (ok == true && login.LoggedInUser is not null)
            {
                var user = login.LoggedInUser;

                Window next = user.Role == "admin"
                            ? new MainWindow()
                            : new UserPanelWindow(user);

                Application.Current.MainWindow = next;
                Close();
                next.Show(); // show dashboard for new user
            }
            else
            {
                Application.Current.Shutdown();
            }
        }
    }
}
