using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AeroSpaceEFB.Models;

public class ChecklistItem : INotifyPropertyChanged
{
    public string Title { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;

    private bool _isChecked;
    public bool IsChecked
    {
        get => _isChecked;
        set
        {
            if (_isChecked != value)
            {
                _isChecked = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}