namespace AeroSpaceEFB;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Startet direkt mit dem Ladebildschirm
        MainPage = new SplashPage();
    }
}