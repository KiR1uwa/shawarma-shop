// =====================================
// File: App.xaml.cs
// =====================================

using System.Windows;
using Authentication;

namespace ShawarmaShop;

/// <summary>
/// Enumerates the initial action the application should perform on startup.
/// </summary>
public enum AppStartAction
{
    /// <summary>Show the <c>LoginWindow</c>.</summary>
    Login,
    /// <summary>Show the <c>RegisterWindow</c>.</summary>
    Register
}

/// <summary>
/// Application bootstrapper that resolves the first window to display depending on <see cref="AppStartAction"/>.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Gets or sets the startup action that determines which window appears first.
    /// Defaults to <see cref="AppStartAction.Login"/>.
    /// </summary>
    public static AppStartAction StartAction { get; set; } = AppStartAction.Login;

    /// <inheritdoc/>
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
                    // User cancelled login – terminate the application gracefully.
                    Shutdown();
                    return;
                }
                break;
        }

        MainWindow = startWindow;
        startWindow.Show();
    }
}