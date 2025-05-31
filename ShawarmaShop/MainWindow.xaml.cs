using System.Windows;

namespace ShawarmaShop
{
    public partial class MainWindow : Window
    {
        private readonly MenuControl menuControl = new();
        private readonly ClientsControl clientsControl = new();
        private readonly OrdersControl ordersControl = new();

        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Content = menuControl;
        }

        private void BtnMenu_Click(object sender, RoutedEventArgs e) => MainFrame.Content = menuControl;
        private void BtnClients_Click(object sender, RoutedEventArgs e) => MainFrame.Content = clientsControl;
        private void BtnOrders_Click(object sender, RoutedEventArgs e) => MainFrame.Content = ordersControl;

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
    }
}
