using System;
using AeroSpaceEFB.ViewModels;
using Microsoft.Maui.Controls;

namespace AeroSpaceEFB;

public partial class ScratchpadPage : ContentPage
{
    public ScratchpadPage()
    {
        InitializeComponent();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // Automatisches Speichern beim Verlassen der Seite
        if (BindingContext is ViewModels.ScratchpadViewModel vm)
        {
            vm.SaveNotes();
        }
    }

    private async void OnBackToMenuClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private void OnExportClicked(object sender, EventArgs e)
    {
        if (BindingContext is ScratchpadViewModel vm)
        {
            vm.ExportScratchpadSummaryCommand.Execute(null);
        }
    }
}