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
                MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!Regex.IsMatch(phone, @"^\+?[1-9]\d{8,14}$"))
            {
                MessageBox.Show("Invalid phone number.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Invalid email address.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (dbContext.Users.Any(u => u.Username == username))
            {
                MessageBox.Show("Username already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newUser = new User
            {
                Username = username,
                PasswordHash = PasswordHasher.Hash(password),
                Phone = phone,
                Email = email,
                Role = UserRoles.User
            };

            dbContext.Users.Add(newUser);
            dbContext.SaveChanges();

            MessageBox.Show("Registration successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            var login = new LoginWindow();
            login.Show();
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => Close();

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            var login = new LoginWindow();
            login.Show();
            Close();
        }

        protected override void OnClosed(System.EventArgs e)
        {
            dbContext.Dispose();
            base.OnClosed(e);
        }
    }
}
