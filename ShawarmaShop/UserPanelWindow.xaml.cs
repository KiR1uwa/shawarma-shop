using System.Windows;
using Authentication;

namespace ShawarmaShop;

public partial class UserPanelWindow : Window
{
    private readonly User user;

    public UserPanelWindow(User user)
    {
        InitializeComponent();
        this.user = user;
        welcomeText.Text = $"Welcome, {user.Username}!";
    }
}
