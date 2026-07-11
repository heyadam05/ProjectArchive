using ProjectArchive.Models;

namespace ProjectArchive.Services;

public sealed class MemorySettingsStorageService : ISettingsStorageService
{
    public AppSettings SavedSettings { get; private set; } = new();

    public AppSettings Load()
    {
        return new AppSettings
        {
            IsDarkTheme = SavedSettings.IsDarkTheme,
            AutoSaveEnabled = SavedSettings.AutoSaveEnabled,
            DailyBackupsEnabled = SavedSettings.DailyBackupsEnabled,
            AccentColor = SavedSettings.AccentColor
        };
    }

    public void Save(AppSettings settings)
    {
        SavedSettings = new AppSettings
        {
            IsDarkTheme = settings.IsDarkTheme,
            AutoSaveEnabled = settings.AutoSaveEnabled,
            DailyBackupsEnabled = settings.DailyBackupsEnabled,
            AccentColor = settings.AccentColor
        };
    }
}
