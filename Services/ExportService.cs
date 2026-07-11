using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using ProjectArchive.Models;

namespace ProjectArchive.Services;

public sealed class ExportService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public async Task ExportJsonAsync(IEnumerable<ArchiveProject> projects, string path)
    {
        var snapshots = projects.Select(ProjectExportSnapshot.FromProject).ToList();
        await File.WriteAllTextAsync(path, JsonSerializer.Serialize(snapshots, JsonOptions));
    }

    public async Task ExportCsvAsync(IEnumerable<ArchiveProject> projects, string path)
    {
        var builder = new StringBuilder();
        builder.AppendLine("Name,Status,Difficulty,Priority,Category,Rating,Favorite,Archived,Technologies,Tags,Updated");

        foreach (var project in projects)
        {
            var values = new[]
            {
                project.Name,
                project.Status.ToString(),
                project.Difficulty.ToString(),
                project.Priority.ToString(),
                project.Category.ToString(),
                project.Rating.ToString(CultureInfo.InvariantCulture),
                project.IsFavorite.ToString(CultureInfo.InvariantCulture),
                project.IsArchived.ToString(CultureInfo.InvariantCulture),
                string.Join("; ", project.ProjectTechnologies.Select(item => item.Technology.Name)),
                string.Join("; ", project.ProjectTags.Select(item => item.Tag.Name)),
                project.UpdatedAt.ToString("u", CultureInfo.InvariantCulture)
            };

            builder.AppendLine(string.Join(',', values.Select(EscapeCsv)));
        }

        await File.WriteAllTextAsync(path, builder.ToString());
    }

    private static string EscapeCsv(string value)
    {
        if (!value.Contains(',') && !value.Contains('"') && !value.Contains('\n'))
        {
            return value;
        }

        return $"\"{value.Replace("\"", "\"\"", StringComparison.Ordinal)}\"";
    }

    private sealed record ProjectExportSnapshot(
        string Name,
        string Description,
        string Status,
        string Difficulty,
        string Priority,
        string Category,
        int Rating,
        bool IsFavorite,
        bool IsArchived,
        double EstimatedHours,
        double ActualHours,
        string GitHubUrl,
        string DemoUrl,
        IReadOnlyList<string> Technologies,
        IReadOnlyList<string> Tags,
        string Notes,
        DateTime CreatedAt,
        DateTime UpdatedAt)
    {
        public static ProjectExportSnapshot FromProject(ArchiveProject project)
        {
            return new ProjectExportSnapshot(
                project.Name,
                project.Description,
                project.Status.ToString(),
                project.Difficulty.ToString(),
                project.Priority.ToString(),
                project.Category.ToString(),
                project.Rating,
                project.IsFavorite,
                project.IsArchived,
                project.EstimatedHours,
                project.ActualHours,
                project.GitHubUrl,
                project.DemoUrl,
                project.ProjectTechnologies.Select(item => item.Technology.Name).Order().ToList(),
                project.ProjectTags.Select(item => item.Tag.Name).Order().ToList(),
                project.Notes,
                project.CreatedAt,
                project.UpdatedAt);
        }
    }
}
