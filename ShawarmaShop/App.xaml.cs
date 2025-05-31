using System.Windows;
using Authentication;

namespace ShawarmaShop
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            var loginWindow = new LoginWindow();
            bool? result = loginWindow.ShowDialog();

            if (result == true && loginWindow.LoggedInUser is not null)
            {
                var user = loginWindow.LoggedInUser;

                Window nextWindow;

                if (user.Role == "admin")
                    nextWindow = new MainWindow();
                else
                    nextWindow = new UserPanelWindow(user);

                this.MainWindow = nextWindow;
                nextWindow.Show();
            }
            else
            {
                Shutdown();
            }
        }
    }
}
