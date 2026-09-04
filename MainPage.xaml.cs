namespace AeroSpaceEFB;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Startet das Laden erst, sobald die Seite sichtbar ist
        if (BindingContext is ViewModels.MainViewModel vm)
        {
            await vm.LoadAndSyncDataAsync();
        }
    }

    private async void OnBackToMenuClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private void OnToggleScreenLockClicked(object sender, EventArgs e)
    {
        // Blendet die transparente Sperrschicht ein oder aus
        ScreenLockOverlay.IsVisible = !ScreenLockOverlay.IsVisible;
    }
}