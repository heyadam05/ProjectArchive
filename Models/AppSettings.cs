namespace ProjectArchive.Models;

public sealed class AppSettings
{
    public bool IsDarkTheme { get; set; } = true;

    public bool AutoSaveEnabled { get; set; } = true;

    public bool DailyBackupsEnabled { get; set; }

    public string AccentColor { get; set; } = "#3B82F6";
}
