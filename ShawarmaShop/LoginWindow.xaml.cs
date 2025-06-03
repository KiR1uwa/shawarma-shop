using System.Linq;
using System.Windows;
using Authentication;
using ShawarmaShop.Infrastructure.Data;

namespace ShawarmaShop
{
    /// <summary>
    ///     Authentication window that lets a user log in,
    ///     register a new account, or close the application.
    /// </summary>
    public partial class LoginWindow : Window
    {
        /// <summary>
        ///     EF Core database context used to fetch users and verify credentials.
        /// </summary>
        private readonly AppDbContext dbContext = new();

        /// <summary>
        ///     Gets the user that successfully authenticated
        ///     (or <see langword="null"/> if the dialog was canceled).
        /// </summary>
        public User? LoggedInUser { get; private set; }

        /// <summary>
        ///     Initializes the login window and its components.
        /// </summary>
        public LoginWindow() => InitializeComponent();

        /// <summary>
        ///     Validates the entered user name / password and either
        ///     sets <see cref="LoggedInUser"/> or shows an error message.
        /// </summary>
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            // Hide the dialog so the user can’t double-click buttons while we work.

            // NOTE: Showing main window *before* we finish validation looks odd
            // but matches the original flow. Consider refactoring.
            

            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password.Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter username and password.",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            var user = dbContext.Users.FirstOrDefault(u => u.Username == username);

            if (user != null && PasswordHasher.Verify(password, user.PasswordHash))
            {
                LoggedInUser = user;
                DialogResult = true;   // close window with success
            }
            else
            {
                MessageBox.Show("Invalid login or password ❌",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        /// <summary>Closes the window and terminates the application if it was the root.</summary>
        private void BtnExit_Click(object sender, RoutedEventArgs e) => Close();

        /// <summary>Opens the registration dialog so a new user can sign up.</summary>
        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            var reg = new RegisterWindow { Owner = this };
            reg.ShowDialog();
        }

        /// <inheritdoc />
        protected override void OnClosed(System.EventArgs e)
        {
            dbContext.Dispose();
            base.OnClosed(e);
        }
    }
}
