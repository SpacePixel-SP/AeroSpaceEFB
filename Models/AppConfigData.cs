using System.Collections.ObjectModel;

namespace AeroSpaceEFB.Models;

public class AppConfigData
{
    public int DataVersion { get; set; } = 1;
    public ObservableCollection<Aircraft> AircraftList { get; set; } = new();
}