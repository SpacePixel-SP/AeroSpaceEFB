using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AeroSpaceEFB.Models;

public class ScratchpadData : INotifyPropertyChanged
{
    // Flight & Flight Plan Info
    private string _callsign = string.Empty;
    public string Callsign
    {
        get => _callsign;
        set { _callsign = value; OnPropertyChanged(); }
    }

    private string _depIcao = string.Empty;
    public string DepIcao
    {
        get => _depIcao;
        set { _depIcao = value; OnPropertyChanged(); }
    }

    private string _destIcao = string.Empty;
    public string DestIcao
    {
        get => _destIcao;
        set { _destIcao = value; OnPropertyChanged(); }
    }

    // SID Info
    private string _plannedSid = string.Empty;
    public string PlannedSid
    {
        get => _plannedSid;
        set { _plannedSid = value; OnPropertyChanged(); }
    }

    private string _actualSid = string.Empty;
    public string ActualSid
    {
        get => _actualSid;
        set { _actualSid = value; OnPropertyChanged(); }
    }

    // Timings
    private string _plannedOffblockTime = string.Empty;
    public string PlannedOffblockTime
    {
        get => _plannedOffblockTime;
        set { _plannedOffblockTime = value; OnPropertyChanged(); }
    }

    private string _actualOffblockTime = string.Empty;
    public string ActualOffblockTime
    {
        get => _actualOffblockTime;
        set { _actualOffblockTime = value; OnPropertyChanged(); }
    }

    // ATC / Control Data
    private string _squawk = string.Empty;
    public string Squawk
    {
        get => _squawk;
        set { _squawk = value; OnPropertyChanged(); }
    }

    private string _runway = string.Empty;
    public string Runway
    {
        get => _runway;
        set { _runway = value; OnPropertyChanged(); }
    }

    private string _altimeter = string.Empty;
    public string Altimeter
    {
        get => _altimeter;
        set { _altimeter = value; OnPropertyChanged(); }
    }

    private string _atisText = string.Empty;
    public string AtisText
    {
        get => _atisText;
        set { _atisText = value; OnPropertyChanged(); }
    }

    // Clearance & Free Notes
    private string _clearanceText = string.Empty;
    public string ClearanceText
    {
        get => _clearanceText;
        set { _clearanceText = value; OnPropertyChanged(); }
    }

    private string _freeNotes = string.Empty;
    public string FreeNotes
    {
        get => _freeNotes;
        set { _freeNotes = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}