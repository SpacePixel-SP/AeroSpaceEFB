using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AeroSpaceEFB.Models;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace AeroSpaceEFB.ViewModels;

public class SettingsViewModel : INotifyPropertyChanged
{
    private AppSettings _settings = new();
    public AppSettings Settings
    {
        get => _settings;
        set { _settings = value; OnPropertyChanged(); }
    }

    public ICommand ResetAllDataCommand { get; }
    public ICommand SaveSettingsCommand { get; }


    public ICommand ExportDataCommand { get; }
    public ICommand ImportDataCommand { get; }

    // Im Konstruktor hinzufügen:
    // ExportDataCommand = new Command(ExportData);
    // ImportDataCommand = new Command(ImportData);

    private async void ExportData()
    {
        try
        {
            string sourceFile = Path.Combine(FileSystem.AppDataDirectory, "aerospace_efb_data.json");

            if (!File.Exists(sourceFile))
            {
                if (Application.Current?.MainPage != null)
                    await Application.Current.MainPage.DisplayAlert("Export", "Keine lokalen Daten zum Exportieren gefunden.", "OK");
                return;
            }

            // Nutzen des MAUI-Share-Dialogs zum Speichern / Senden der Datei
            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = "AeroSpace EFB Backup exportieren",
                File = new ShareFile(sourceFile)
            });
        }
        catch (Exception ex)
        {
            if (Application.Current?.MainPage != null)
                await Application.Current.MainPage.DisplayAlert("Fehler", $"Export fehlgeschlagen: {ex.Message}", "OK");
        }
    }

    private async void ImportData()
    {
        try
        {
            // Custom FileType für JSON-Dateien definieren
            var jsonFileType = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                { DevicePlatform.WinUI, new[] { ".json" } },
                { DevicePlatform.Android, new[] { "application/json" } },
                { DevicePlatform.iOS, new[] { "public.json" } },
                { DevicePlatform.MacCatalyst, new[] { "public.json" } }
                });

            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Wähle ein AeroSpace EFB Backup (.json)",
                FileTypes = jsonFileType
            });

            if (result != null)
            {
                string destinationFile = Path.Combine(FileSystem.AppDataDirectory, "aerospace_efb_data.json");
                File.Copy(result.FullPath, destinationFile, overwrite: true);

                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Erfolg", "Daten wurden erfolgreich importiert!", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            if (Application.Current?.MainPage != null)
                await Application.Current.MainPage.DisplayAlert("Fehler", $"Import fehlgeschlagen: {ex.Message}", "OK");
        }
    }

    public SettingsViewModel()
    {
        ResetAllDataCommand = new Command(ResetAllData);
        SaveSettingsCommand = new Command(SaveSettings);
        ExportDataCommand = new Command(ExportData);
        ImportDataCommand = new Command(ImportData);

        LoadSettings();
    }

    public void LoadSettings()
    {
        Settings.IsDarkMode = Preferences.Get(nameof(Settings.IsDarkMode), true);
        Settings.AutoSaveEnabled = Preferences.Get(nameof(Settings.AutoSaveEnabled), true);
        Settings.UseUtcTime = Preferences.Get(nameof(Settings.UseUtcTime), true);
        ApplyTheme(Settings.IsDarkMode);
    }

    public void SaveSettings()
    {
        Preferences.Set(nameof(Settings.IsDarkMode), Settings.IsDarkMode);
        Preferences.Set(nameof(Settings.AutoSaveEnabled), Settings.AutoSaveEnabled);
        Preferences.Set(nameof(Settings.UseUtcTime), Settings.UseUtcTime);

        ApplyTheme(Settings.IsDarkMode);
    }

    private void ApplyTheme(bool isDark)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (Application.Current != null)
            {
                Application.Current.UserAppTheme = isDark ? AppTheme.Dark : AppTheme.Light;
            }
        });
    }

    private async void ResetAllData()
    {
        if (Application.Current?.MainPage == null) return;

        bool confirm = await Application.Current.MainPage.DisplayAlert(
            "Werkseinstellungen & Daten-Reset",
            "Möchtest du WIRKLICH alle Flugzeuge, Checklisten und Scratchpad-Daten löschen und auf den Entwickler-Standard zurücksetzen?",
            "Ja, alles löschen",
            "Abbrechen");

        if (confirm)
        {
            try
            {
                // Lösche lokale JSON-Datenbanken
                string dataFile = Path.Combine(FileSystem.AppDataDirectory, "aerospace_efb_data.json");
                string scratchpadFile = Path.Combine(FileSystem.AppDataDirectory, "aerospace_scratchpad.json");

                if (File.Exists(dataFile)) File.Delete(dataFile);
                if (File.Exists(scratchpadFile)) File.Delete(scratchpadFile);

                Preferences.Clear(); // Einstellungen zurücksetzen

                await Application.Current.MainPage.DisplayAlert(
                    "Erfolg",
                    "Alle lokalen Daten wurden zurückgesetzt. Die App wird beim nächsten Seitenaufruf die Standard-Checklisten neu laden.",
                    "OK");

                LoadSettings();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Fehler", $"Fehler beim Zurücksetzen: {ex.Message}", "OK");
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}