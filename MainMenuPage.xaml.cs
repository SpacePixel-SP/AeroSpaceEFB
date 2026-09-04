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
}