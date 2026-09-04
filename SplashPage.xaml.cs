namespace AeroSpaceEFB;

public partial class SplashPage : ContentPage
{
    public SplashPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // 1. Initialer Status
        StatusLabel.Text = "Initialisiere System...";
        await LoadingBar.ProgressTo(0.2, 300, Easing.Linear);

        // 2. JSON-Ladevorgang ankündigen
        StatusLabel.Text = "Lade aerospace_efb_data.json...";
        await LoadingBar.ProgressTo(0.6, 500, Easing.Linear);

        // Hier entsteht eine kurze Pause fürs Auge, während die App lädt
        await Task.Delay(1600);

        // 3. Abschluss
        StatusLabel.Text = "Fertiggestellt!";
        await LoadingBar.ProgressTo(1.0, 300, Easing.Linear);

        await Task.Delay(600);

        // 4. Weiterleitung zum Hauptmenü (NavigationPage)
        Application.Current.MainPage = new NavigationPage(new MainMenuPage())
        {
            BarBackgroundColor = Color.FromArgb("#1E293B"),
            BarTextColor = Colors.White
        };
    }
}