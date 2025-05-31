using System.Linq;
using System.Windows;
using Authentication;

namespace ShawarmaShop
{
    public partial class RegisterWindow : Window
    {
        private readonly AppDbContext dbContext = new();

        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password.Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Enter username and password", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (dbContext.Users.Any(u => u.Username == username))
            {
                MessageBox.Show("Username already exists", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newUser = new User
            {
                Username = username,
                PasswordHash = PasswordHasher.Hash(password),
                Role = "user"
            };

            dbContext.Users.Add(newUser);
            dbContext.SaveChanges();

            MessageBox.Show("Registration successful ✅", "Success");
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            var reg = new LoginWindow { Owner = this };
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
