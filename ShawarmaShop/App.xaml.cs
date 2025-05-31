using System.Windows;
using Authentication;

namespace ShawarmaShop;

public enum AppStartAction
{
    Login,
    Register
}

public partial class App : Application
{
    public static AppStartAction StartAction { get; set; } = AppStartAction.Login;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        ShutdownMode = ShutdownMode.OnExplicitShutdown;

        Window startWindow;

        switch (StartAction)
        {
            case AppStartAction.Register:
                startWindow = new RegisterWindow();
                break;

            case AppStartAction.Login:
            default:
                var loginWindow = new LoginWindow();
                var result = loginWindow.ShowDialog();

                if (result == true && loginWindow.LoggedInUser is not null)
                {
                    var user = loginWindow.LoggedInUser;

                    if (user.Role == "admin")
                        startWindow = new MainWindow();
                    else
                        startWindow = new UserPanelWindow(user);
                }
                else
                {
                    Shutdown();
                    return;
                }
                break;
        }

        MainWindow = startWindow;
        startWindow.Show();
    }
}
