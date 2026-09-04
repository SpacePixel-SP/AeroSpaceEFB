using System.Text.Json;
using AeroSpaceEFB.Models;

namespace AeroSpaceEFB.Services;

public class JsonChecklistService
{
    private readonly string _filePath = Path.Combine(FileSystem.AppDataDirectory, "aerospace_checklist.json");

    public async Task SaveChecklistAsync(Checklist checklist)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(checklist, options);
        await File.WriteAllTextAsync(_filePath, json);
    }

    public async Task<Checklist?> LoadChecklistAsync()
    {
        if (!File.Exists(_filePath))
            return null;

        string json = await File.ReadAllTextAsync(_filePath);
        return JsonSerializer.Deserialize<Checklist>(json);
    }
}