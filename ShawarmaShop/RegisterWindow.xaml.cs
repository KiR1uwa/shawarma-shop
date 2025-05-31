using System.Linq;
using System.Text.RegularExpressions;
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
            string phone = txtPhone.Text.Trim();
            string email = txtEmail.Text.Trim();

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Please fill in all fields", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var phoneRx = new Regex(@"^\+?[1-9]\d{8,14}$");
            if (!phoneRx.IsMatch(phone))
            {
                MessageBox.Show("Invalid phone number", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var emailRx = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!emailRx.IsMatch(email))
            {
                MessageBox.Show("Invalid e-mail address", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                Phone = phone,
                Email = email,
                Role = "user"
            };

            dbContext.Users.Add(newUser);
            dbContext.SaveChanges();

            MessageBox.Show("Registration successful", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            if (Owner is LoginWindow)
            {
                DialogResult = true;
            }
            else
            {
                var login = new LoginWindow();
                Application.Current.MainWindow = login;
                login.Show();
                Close();
            }
            this.Hide();
            var reg = new LoginWindow();
            Owner = reg.Owner;
            reg.ShowDialog();
            this.Show();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => Close();

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            var reg = new LoginWindow();
            Owner = reg.Owner;
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
