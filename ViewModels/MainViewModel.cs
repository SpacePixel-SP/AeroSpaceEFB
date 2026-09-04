using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using AeroSpaceEFB.Models;
using Microsoft.Maui.Storage;

namespace AeroSpaceEFB.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    // Pfad zur Speicherdatei im lokalen App-Datenverzeichnis des Clients
    private readonly string _filePath = Path.Combine(FileSystem.AppDataDirectory, "aerospace_efb_data.json");

    public ObservableCollection<Aircraft> AircraftList { get; set; } = new();
    public int CurrentLoadedVersion { get; private set; } = 0;

    private Aircraft? _selectedAircraft;
    public Aircraft? SelectedAircraft
    {
        get => _selectedAircraft;
        set
        {
            if (_selectedAircraft != value)
            {
                _selectedAircraft = value;
                OnPropertyChanged();

                if (_selectedAircraft?.Checklists.Count > 0)
                {
                    SelectedChecklist = _selectedAircraft.Checklists[0];
                }
                else
                {
                    SelectedChecklist = null;
                }
            }
        }
    }

    private Checklist? _selectedChecklist;
    public Checklist? SelectedChecklist
    {
        get => _selectedChecklist;
        set
        {
            if (_selectedChecklist != value)
            {
                _selectedChecklist = value;
                OnPropertyChanged();
            }
        }
    }

    // Eingabefelder für neue Elemente
    private string _newAircraftName = string.Empty;
    public string NewAircraftName
    {
        get => _newAircraftName;
        set { _newAircraftName = value; OnPropertyChanged(); }
    }

    private string _newChecklistTitle = string.Empty;
    public string NewChecklistTitle
    {
        get => _newChecklistTitle;
        set { _newChecklistTitle = value; OnPropertyChanged(); }
    }

    private string _newItemTitle = string.Empty;
    public string NewItemTitle
    {
        get => _newItemTitle;
        set { _newItemTitle = value; OnPropertyChanged(); }
    }

    private string _newItemAction = string.Empty;
    public string NewItemAction
    {
        get => _newItemAction;
        set { _newItemAction = value; OnPropertyChanged(); }
    }

    // Commands
    public ICommand AddAircraftCommand { get; }
    public ICommand AddChecklistCommand { get; }
    public ICommand AddItemCommand { get; }

    public ICommand DeleteAircraftCommand { get; }
    public ICommand DeleteChecklistCommand { get; }
    public ICommand DeleteItemCommand { get; }

    public ICommand ResetCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand LoadCommand { get; }

    private bool _isLoading = true;
    public bool IsLoading
    {
        get => _isLoading;
        set { _isLoading = value; OnPropertyChanged(); }
    }

    public MainViewModel()
    {
        //TEMPORÄR ALTE DATEIEN IMMER LÖSCHEN!!!
        //if (File.Exists(_filePath)) File.Delete(_filePath);

        // Commands initialisieren
        AddAircraftCommand = new Command(AddAircraft);
        AddChecklistCommand = new Command(AddChecklist);
        AddItemCommand = new Command(AddItem);

        DeleteAircraftCommand = new Command<Aircraft>(DeleteAircraft);
        DeleteChecklistCommand = new Command<Checklist>(DeleteChecklist);
        DeleteItemCommand = new Command<ChecklistItem>(DeleteItem);

        ResetCommand = new Command(ResetCurrentChecklist);
        SaveCommand = new Command(SaveToJson);
        LoadCommand = new Command(async () => await LoadAndSyncDataAsync());

        // Beim Start Daten laden & mit Entwickler-Updates synchronisieren
        
    }

    /// <summary>
    /// Lädt die Daten des Benutzers und führt neue Checklisten aus Updates automatisch zusammen.
    /// </summary>
    public async Task LoadAndSyncDataAsync()
    {
        IsLoading = true;

        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Kurze Kunstpause (100ms), damit die UI Zeit hat, das Lade-Overlay erst einmal anzuzeigen
            await Task.Delay(100);

            AppConfigData? devData = await LoadDevDefaultDataAsync();
            AppConfigData? userData = LoadUserData();

            AircraftList.Clear();

            if (userData == null)
            {
                if (devData != null)
                {
                    foreach (var a in devData.AircraftList) AircraftList.Add(a);
                    CurrentLoadedVersion = devData.DataVersion;
                }
            }
            else
            {
                foreach (var a in userData.AircraftList) AircraftList.Add(a);
                CurrentLoadedVersion = userData.DataVersion;

                if (devData != null && devData.DataVersion > userData.DataVersion)
                {
                    MergeDevDataIntoUserData(devData);
                    CurrentLoadedVersion = devData.DataVersion;
                }
            }

            if (AircraftList.Count > 0)
            {
                SelectedAircraft = AircraftList[0];
            }

            SaveToJson();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[EFB Sync Fehler]: {ex.Message}");
        }
        finally
        {
            // 2. Mindest-Anzeigedauer berechnen (z.B. 1000 ms = 1 Sekunde)
            int minDisplayTimeMs = 5000;
            int elapsedMs = (int)stopwatch.ElapsedMilliseconds;

            // Falls das Laden schneller war als die Mindestzeit, warten wir den Rest ab
            if (elapsedMs < minDisplayTimeMs)
            {
                await Task.Delay(minDisplayTimeMs - elapsedMs);
            }

            // Erst jetzt wird der Ladebildschirm ausgeblendet
            IsLoading = false;
        }
    }

    private async Task<AppConfigData?> LoadDevDefaultDataAsync()
    {
        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("default_checklists.json");
            using var reader = new StreamReader(stream);
            string json = await reader.ReadToEndAsync();
            return JsonSerializer.Deserialize<AppConfigData>(json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Fehler beim Laden der Dev-JSON: {ex.Message}");
            return null;
        }
    }

    private AppConfigData? LoadUserData()
    {
        if (!File.Exists(_filePath)) return null;

        string json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<AppConfigData>(json);
    }

    /// <summary>
    /// Fügt neue Entwickler-Flugzeuge & Checklisten hinzu, ohne benutzerdefinierte Daten zu überschreiben.
    /// </summary>
    private void MergeDevDataIntoUserData(AppConfigData devData)
    {
        foreach (var devAircraft in devData.AircraftList)
        {
            var userAircraft = AircraftList.FirstOrDefault(a => a.TailNumberOrModel == devAircraft.TailNumberOrModel);

            if (userAircraft == null)
            {
                // Flugzeug ist komplett neu vom Entwickler -> Hinzufügen
                AircraftList.Add(devAircraft);
            }
            else
            {
                // Flugzeug existiert bereits -> Checklisten abgleichen
                foreach (var devChecklist in devAircraft.Checklists)
                {
                    var userChecklist = userAircraft.Checklists.FirstOrDefault(c => c.Title == devChecklist.Title);

                    if (userChecklist == null)
                    {
                        // Checkliste ist neu -> Hinzufügen
                        userAircraft.Checklists.Add(devChecklist);
                    }
                }
            }
        }
    }

    // --- JSON SPEICHERN ---
    public void SaveToJson()
    {
        // Prüfen, ob AutoSave in den Einstellungen aktiviert ist
        bool autoSave = Preferences.Get("AutoSaveEnabled", true);

        if (!autoSave)
        {
            return;
        }

        try
        {
            string filePath = System.IO.Path.Combine(Microsoft.Maui.Storage.FileSystem.AppDataDirectory, "aerospace_efb_data.json");

            var config = new AppConfigData
            {
                DataVersion = CurrentLoadedVersion,
                // KORREKTUR: Kein .ToList(), sondern direkt zuweisen
                // Mit ?? new() sichern wir uns zusätzlich gegen CS8602 (Nullverweis) ab
                AircraftList = AircraftList ?? new System.Collections.ObjectModel.ObservableCollection<Aircraft>()
            };

            string json = System.Text.Json.JsonSerializer.Serialize(config, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[EFB Save Error]: {ex.Message}");
        }
    }

    // --- HINZUFÜGEN ---
    private void AddAircraft()
    {
        if (string.IsNullOrWhiteSpace(NewAircraftName)) return;

        var newPlane = new Aircraft { TailNumberOrModel = NewAircraftName };
        AircraftList.Add(newPlane);
        SelectedAircraft = newPlane;
        NewAircraftName = string.Empty;

        SaveToJson();
    }

    private void AddChecklist()
    {
        if (SelectedAircraft == null || string.IsNullOrWhiteSpace(NewChecklistTitle)) return;

        var newList = new Checklist { Title = NewChecklistTitle };
        SelectedAircraft.Checklists.Add(newList);
        SelectedChecklist = newList;
        NewChecklistTitle = string.Empty;

        SaveToJson();
    }

    private void AddItem()
    {
        if (SelectedChecklist == null || string.IsNullOrWhiteSpace(NewItemTitle)) return;

        SelectedChecklist.Items.Add(new ChecklistItem
        {
            Title = NewItemTitle,
            Action = NewItemAction,
            IsChecked = false
        });

        NewItemTitle = string.Empty;
        NewItemAction = string.Empty;

        SaveToJson();
    }

    // --- LÖSCHEN ---
    private async void DeleteAircraft(Aircraft? aircraft)
    {
        if (aircraft == null || Application.Current?.MainPage == null) return;

        bool confirm = await Application.Current.MainPage.DisplayAlert(
            "Flugzeug löschen",
            $"Möchtest du '{aircraft.TailNumberOrModel}' wirklich unwiderruflich löschen?",
            "Löschen",
            "Abbrechen");

        if (!confirm) return;

        AircraftList.Remove(aircraft);

        if (SelectedAircraft == aircraft)
        {
            SelectedAircraft = AircraftList.FirstOrDefault();
        }

        SaveToJson();
    }

    private async void DeleteChecklist(Checklist? checklist)
    {
        if (checklist == null || SelectedAircraft == null || Application.Current?.MainPage == null) return;

        bool confirm = await Application.Current.MainPage.DisplayAlert(
            "Checkliste löschen",
            $"Möchtest du die Checkliste '{checklist.Title}' wirklich löschen?",
            "Löschen",
            "Abbrechen");

        if (!confirm) return;

        SelectedAircraft.Checklists.Remove(checklist);

        if (SelectedChecklist == checklist)
        {
            SelectedChecklist = SelectedAircraft.Checklists.FirstOrDefault();
        }

        SaveToJson();
    }

    private async void DeleteItem(ChecklistItem? item)
    {
        if (item == null || SelectedChecklist == null || Application.Current?.MainPage == null) return;

        bool confirm = await Application.Current.MainPage.DisplayAlert(
            "Eintrag löschen",
            $"Möchtest du '{item.Title}' aus der Checkliste entfernen?",
            "Löschen",
            "Abbrechen");

        if (!confirm) return;

        SelectedChecklist.Items.Remove(item);
        SelectedChecklist.UpdateCompletionStatus();

        SaveToJson();
    }

    // --- RESET ---
    private void ResetCurrentChecklist()
    {
        if (SelectedChecklist == null) return;

        foreach (var item in SelectedChecklist.Items)
        {
            item.IsChecked = false;
        }

        SelectedChecklist.UpdateCompletionStatus();
        SaveToJson();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}