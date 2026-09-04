using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AeroSpaceEFB.Models;

public class Checklist : INotifyPropertyChanged
{
    private string _title = string.Empty;
    public string Title
    {
        get => _title;
        set { _title = value; OnPropertyChanged(); }
    }

    private bool _isEmergency = false;
    public bool IsEmergency
    {
        get => _isEmergency;
        set { _isEmergency = value; OnPropertyChanged(); }
    }

    public ObservableCollection<ChecklistItem> Items { get; set; } = new();

    private bool _isCompleted;
    public bool IsCompleted
    {
        get => _isCompleted;
        set { _isCompleted = value; OnPropertyChanged(); }
    }

    public void UpdateCompletionStatus()
    {
        IsCompleted = Items.Count > 0 && Items.All(item => item.IsChecked);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}