using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Controls;

namespace ShawarmaShop
{
    public partial class MainWindow : Window
    {
        private MenuControl menuControl = new MenuControl();
        private ClientsControl clientsControl = new ClientsControl();
        private OrdersControl ordersControl = new OrdersControl();

        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Content = menuControl;
        }

        private void BtnMenu_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = menuControl;
        }

        private void BtnClients_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = clientsControl;
        }

        private void BtnOrders_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = ordersControl;
        }
    }
}
