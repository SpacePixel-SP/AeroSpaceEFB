namespace AeroSpaceEFB;

public partial class MainMenuPage : ContentPage
{
    public MainMenuPage()
    {
        InitializeComponent();
    }

    private async void OnChecklistClicked(object sender, EventArgs e)
    {
        // Navigiert zur Checklisten-Seite
        await Navigation.PushAsync(new MainPage());
    }

    private async void OnOpenScratchpadClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ScratchpadPage());
    }

    private async void OnOpenCalculationsClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CalculationsPage());
    }

    private async void OnOpenSettingsClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SettingsPage());
    }
}