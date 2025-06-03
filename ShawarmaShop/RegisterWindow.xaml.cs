// =============================================================
// File: RegisterWindow.xaml.cs
// =============================================================

using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using Authentication;
using ShawarmaShop.Infrastructure.Data;

namespace ShawarmaShop
{
    /// <summary>
    ///     Interaction logic for <c>RegisterWindow</c>.
    ///     Presents a simple form that allows a new user to sign up by providing
    ///     a username, password, phone number and e‑mail address. Basic client‑side
    ///     validation is performed before persisting the new <see cref="User"/> to
    ///     the underlying SQLite database.
    /// </summary>
    public partial class RegisterWindow : Window
    {
        /// <summary>
        ///     EF Core database context. Lifetime is bound to the window and it is
        ///     disposed in <see cref="OnClosed"/>.
        /// </summary>
        private readonly AppDbContext dbContext = new();

        /// <summary>
        ///     Initializes a new instance of the <see cref="RegisterWindow"/> class and
        ///     loads its XAML components.
        /// </summary>
        public RegisterWindow() => InitializeComponent();

        /// <summary>
        ///     Validates form fields and, if successful, creates a new <see cref="User"/>
        ///     entity, saves it to the database and redirects the user back to the login
        ///     window.
        /// </summary>
        /// <param name="sender">Button that initiated the action.</param>
        /// <param name="e">Event‑args (unused).</param>
        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            // Extract raw values ------------------------------------------------
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password.Trim();
            string phone = txtPhone.Text.Trim();
            string email = txtEmail.Text.Trim();

            // Validate required fields ----------------------------------------
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Please fill in all fields.",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            // Validate phone (E.164 minimal) ----------------------------------
            if (!Regex.IsMatch(phone, @"^\+?[1-9]\d{8,14}$"))
            {
                MessageBox.Show("Invalid phone number.",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            // Validate e‑mail --------------------------------------------------
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Invalid email address.",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            // Unique username check -------------------------------------------
            if (dbContext.Users.Any(u => u.Username == username))
            {
                MessageBox.Show("Username already exists.",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            // Persist the new user -------------------------------------------
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

            MessageBox.Show("Registration successful!",
                            "Success",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

            // Navigate back to login ------------------------------------------
            Hide();
            var login = new LoginWindow { Owner = this };
            login.ShowDialog();

            // When login window closes we shut down this one too
            Close();
        }

        /// <summary>
        ///     Cancels the registration workflow and closes the window without saving
        ///     any changes.
        /// </summary>
        private void BtnCancel_Click(object sender, RoutedEventArgs e) => Close();

        /// <summary>
        ///     Returns to the login screen without registering a new account. Unlike
        ///     <see cref="BtnCancel_Click"/>, this keeps the application running and
        ///     simply hides the current dialog.
        /// </summary>
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            var login = new LoginWindow { Owner = this };
            login.ShowDialog();
        }

        /// <summary>
        ///     Releases unmanaged resources and disposes the EF Core context when the
        ///     window is closed.
        /// </summary>
        /// <param name="e">Event‑args supplied by WPF.</param>
        protected override void OnClosed(System.EventArgs e)
        {
            dbContext.Dispose();
            base.OnClosed(e);
        }
    }
}
