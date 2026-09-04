namespace AeroSpaceEFB;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // 1. Theme-Einstellung direkt beim Start auslesen
        bool isDark = Preferences.Get("IsDarkMode", true);
        UserAppTheme = isDark ? AppTheme.Dark : AppTheme.Light;

        MainPage = new NavigationPage(new SplashPage());
    }
}