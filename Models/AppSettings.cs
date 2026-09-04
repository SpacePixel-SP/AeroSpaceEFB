using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AeroSpaceEFB.Models;

public class AppSettings : INotifyPropertyChanged
{
    private bool _isDarkMode = true;
    public bool IsDarkMode
    {
        get => _isDarkMode;
        set { _isDarkMode = value; OnPropertyChanged(); }
    }

    private bool _autoSaveEnabled = true;
    public bool AutoSaveEnabled
    {
        get => _autoSaveEnabled;
        set { _autoSaveEnabled = value; OnPropertyChanged(); }
    }

    private bool _useUtcTime = true;
    public bool UseUtcTime
    {
        get => _useUtcTime;
        set { _useUtcTime = value; OnPropertyChanged(); }
    }

    private int _scratchpadMinDisplayTime = 1000;
    public int ScratchpadMinDisplayTime
    {
        get => _scratchpadMinDisplayTime;
        set { _scratchpadMinDisplayTime = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}