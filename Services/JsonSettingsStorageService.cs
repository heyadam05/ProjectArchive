using System.IO;
using System.Text.Json;
using ProjectArchive.Models;

namespace ProjectArchive.Services;

public sealed class JsonSettingsStorageService : ISettingsStorageService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public AppSettings Load()
    {
        if (!File.Exists(SettingsPath))
        {
            return new AppSettings();
        }

        var json = File.ReadAllText(SettingsPath);
        return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
    }

    public void Save(AppSettings settings)
    {
        Directory.CreateDirectory(SettingsDirectory);
        File.WriteAllText(SettingsPath, JsonSerializer.Serialize(settings, JsonOptions));
    }

    private static string SettingsDirectory => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "ProjectArchive");

    private static string SettingsPath => Path.Combine(SettingsDirectory, "settings.json");
}
