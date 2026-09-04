using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AeroSpaceEFB.Models;

public class Checklist : INotifyPropertyChanged
{
    public string Title { get; set; } = string.Empty;

    private ObservableCollection<ChecklistItem> _items = new();
    public ObservableCollection<ChecklistItem> Items
    {
        get => _items;
        set
        {
            if (_items != value)
            {
                if (_items != null)
                {
                    _items.CollectionChanged -= OnItemsCollectionChanged;
                    foreach (var item in _items) item.PropertyChanged -= OnItemPropertyChanged;
                }

                _items = value;

                if (_items != null)
                {
                    _items.CollectionChanged += OnItemsCollectionChanged;
                    foreach (var item in _items) item.PropertyChanged += OnItemPropertyChanged;
                }

                OnPropertyChanged();
                UpdateCompletionStatus();
            }
        }
    }

    private bool _isCompleted;
    public bool IsCompleted
    {
        get => _isCompleted;
        private set
        {
            if (_isCompleted != value)
            {
                _isCompleted = value;
                OnPropertyChanged();
            }
        }
    }

    public Checklist()
    {
        _items.CollectionChanged += OnItemsCollectionChanged;
    }

    private void OnItemsCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (ChecklistItem item in e.NewItems)
                item.PropertyChanged += OnItemPropertyChanged;
        }
        if (e.OldItems != null)
        {
            foreach (ChecklistItem item in e.OldItems)
                item.PropertyChanged -= OnItemPropertyChanged;
        }
        UpdateCompletionStatus();
    }

    private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ChecklistItem.IsChecked))
        {
            UpdateCompletionStatus();
        }
    }

    public void UpdateCompletionStatus()
    {
        // Eine Checkliste gilt als erledigt, wenn sie mindestens 1 Item hat und ALLE Items abgehakt sind
        IsCompleted = Items.Count > 0 && Items.All(item => item.IsChecked);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}