using System.Collections.ObjectModel;

namespace AeroSpaceEFB.Models;

public class Aircraft
{
    public string TailNumberOrModel { get; set; } = "Cessna 172";
    public ObservableCollection<Checklist> Checklists { get; set; } = new();
}