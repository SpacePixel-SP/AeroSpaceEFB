using System;
using Microsoft.Maui.Controls;

namespace AeroSpaceEFB;

public partial class CalculationsPage : ContentPage
{
    public CalculationsPage()
    {
        InitializeComponent();
    }

    private async void OnBackToMenuClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}