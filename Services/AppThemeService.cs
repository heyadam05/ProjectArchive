using System.Windows;
using System.Windows.Media;

namespace ProjectArchive.Services;

public sealed class AppThemeService : IThemeService
{
    public void ApplyTheme(bool isDarkTheme, string accentColor)
    {
        var resources = Application.Current?.Resources;

        if (resources is null)
        {
            return;
        }

        SetBrush(resources, "BackgroundBrush", isDarkTheme ? "#181818" : "#F5F7FB");
        SetBrush(resources, "SidebarBrush", isDarkTheme ? "#202020" : "#FFFFFF");
        SetBrush(resources, "CardBrush", isDarkTheme ? "#2B2B2B" : "#FFFFFF");
        SetBrush(resources, "InputBrush", isDarkTheme ? "#242424" : "#F8FAFC");
        SetBrush(resources, "TextBrush", isDarkTheme ? "#FFFFFF" : "#111827");
        SetBrush(resources, "SecondaryTextBrush", isDarkTheme ? "#B8B8B8" : "#64748B");
        SetBrush(resources, "BorderBrushSoft", isDarkTheme ? "#3A3A3A" : "#CBD5E1");
        SetBrush(resources, "TableAlternateBrush", isDarkTheme ? "#303030" : "#F1F5F9");
        SetBrush(resources, "TableHeaderBrush", isDarkTheme ? "#353535" : "#E2E8F0");
        SetBrush(resources, "SecondaryButtonBrush", isDarkTheme ? "#3A3A3A" : "#E2E8F0");
        SetBrush(resources, "SecondaryButtonTextBrush", isDarkTheme ? "#FFFFFF" : "#111827");
        SetBrush(resources, "PrimaryBrush", accentColor);
    }

    private static void SetBrush(ResourceDictionary resources, string key, string color)
    {
        resources[key] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
    }
}
