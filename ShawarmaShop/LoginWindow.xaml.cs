using System.Linq;
using System.Windows;
using Authentication;

namespace ShawarmaShop
{
    public partial class LoginWindow : Window
    {

        private readonly AppDbContext dbContext = new();

        public User? LoggedInUser { get; private set; }

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow = this;

            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password.Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter username and password.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = dbContext.Users.FirstOrDefault(u => u.Username == username);

            if (user != null && PasswordHasher.Verify(password, user.PasswordHash))
            {
                LoggedInUser = user;
                DialogResult = true;
                
            }
            else
            {
                MessageBox.Show("Invalid login or password ❌", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            var reg = new RegisterWindow { Owner = this };
            reg.ShowDialog();
            this.Show();
        }

        protected override void OnClosed(System.EventArgs e)
        {
            dbContext.Dispose();
            base.OnClosed(e);
        }
    }
}
