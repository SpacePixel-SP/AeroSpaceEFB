using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;
using AeroSpaceEFB.Models;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace AeroSpaceEFB.ViewModels;

public class ScratchpadViewModel : INotifyPropertyChanged
{
    private readonly string _filePath = Path.Combine(FileSystem.AppDataDirectory, "aerospace_scratchpad.json");

    private ScratchpadData _notesData = new();
    public ScratchpadData NotesData
    {
        get => _notesData;
        set { _notesData = value; OnPropertyChanged(); }
    }

    public ICommand ClearAllCommand { get; }
    public ICommand AppendTemplateCommand { get; }
    public ICommand SetActualOffblockNowCommand { get; }

    public ScratchpadViewModel()
    {
        ClearAllCommand = new Command(ClearAll);
        AppendTemplateCommand = new Command<string>(AppendTemplate);
        SetActualOffblockNowCommand = new Command(SetActualOffblockNow);

        ExportScratchpadSummaryCommand = new Command(ExportScratchpadSummary);

        LoadNotes();
    }

    public void SaveNotes()
    {
        try
        {
            string json = JsonSerializer.Serialize(NotesData);
            File.WriteAllText(_filePath, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Scratchpad Save Error: {ex.Message}");
        }
    }

    private void LoadNotes()
    {
        try
        {
            if (File.Exists(_filePath))
            {
                string json = File.ReadAllText(_filePath);
                var data = JsonSerializer.Deserialize<ScratchpadData>(json);
                if (data != null) NotesData = data;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Scratchpad Load Error: {ex.Message}");
        }
    }

    private async void ClearAll()
    {
        if (Application.Current?.MainPage == null) return;

        bool confirm = await Application.Current.MainPage.DisplayAlert(
            "Scratchpad leeren",
            "Möchtest du alle eingetragenen Notizen löschen?",
            "Löschen",
            "Abbrechen");

        if (confirm)
        {
            NotesData = new ScratchpadData();
            SaveNotes();
        }
    }

    private void SetActualOffblockNow()
    {
        // Auslesen der Einstellung aus den Preferences
        bool useUtc = Preferences.Get("UseUtcTime", true);

        if (useUtc)
        {
            NotesData.ActualOffblockTime = DateTime.UtcNow.ToString("HH:mm") + "Z";
        }
        else
        {
            NotesData.ActualOffblockTime = DateTime.Now.ToString("HH:mm") + " L";
        }

        SaveNotes();
    }

    public ICommand ExportScratchpadSummaryCommand { get; }

    // Im Konstruktor:
    // ExportScratchpadSummaryCommand = new Command(ExportScratchpadSummary);

    private async void ExportScratchpadSummary()
    {
        try
        {
            string summary = $"""
        ==================================================
        AEROSPACE EFB - ATC SCRATCHPAD SUMMARY
        Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
        ==================================================

        FLIGHT IDENTIFICATION:
        Callsign:      {NotesData.Callsign}
        Dep / Dest:    {NotesData.DepIcao} -> {NotesData.DestIcao}
        Planned SID:   {NotesData.PlannedSid}
        Cleared SID:   {NotesData.ActualSid}

        TIMINGS:
        EOBT (Plan):   {NotesData.PlannedOffblockTime}
        AOBT (Actual): {NotesData.ActualOffblockTime}

        ATC CLEARANCE & CONTROL:
        Squawk:        {NotesData.Squawk}
        Runway:        {NotesData.Runway}
        QNH / Alt:     {NotesData.Altimeter}
        ATIS Info:     {NotesData.AtisText}

        C.R.A.F.T. CLEARANCE:
        --------------------------------------------------
        {NotesData.ClearanceText}

        TAXI & FREE NOTES:
        --------------------------------------------------
        {NotesData.FreeNotes}
        ==================================================
        """;

            string tempPath = Path.Combine(FileSystem.CacheDirectory, $"Scratchpad_{NotesData.Callsign}_{DateTime.Now:HHmm}.txt");
            File.WriteAllText(tempPath, summary);

            // Bietet Windows-Teilen/Drucken Dialog an
            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = "ATC Scratchpad Export",
                File = new ShareFile(tempPath)
            });
        }
        catch (Exception ex)
        {
            if (Application.Current?.MainPage != null)
                await Application.Current.MainPage.DisplayAlert("Fehler", $"Export fehlgeschlagen: {ex.Message}", "OK");
        }
    }

    private void AppendTemplate(string? templateType)
    {
        if (string.IsNullOrEmpty(templateType)) return;

        switch (templateType)
        {
            case "CRAFT":
                NotesData.ClearanceText = "C: \nR: \nA: \nF: \nT: ";
                break;
            case "TAXI":
                NotesData.FreeNotes += (string.IsNullOrEmpty(NotesData.FreeNotes) ? "" : "\n") + "TAXI: Rwy ... via ...";
                break;
        }
        SaveNotes();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}