using System;
using Microsoft.Maui.Controls;

namespace AeroSpaceEFB;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
    }

    private void OnSettingChanged(object sender, ToggledEventArgs e)
    {
        if (BindingContext is ViewModels.SettingsViewModel vm)
        {
            vm.SaveSettings();
        }
    }

    private async void OnBackToMenuClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}