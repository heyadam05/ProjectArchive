using ProjectArchive.Models;

namespace ProjectArchive.Services;

public sealed class ProjectFilter
{
    public string SearchText { get; init; } = string.Empty;

    public ProjectStatus? Status { get; init; }

    public ProjectDifficulty? Difficulty { get; init; }

    public ProjectCategory? Category { get; init; }

    public ProjectPriority? Priority { get; init; }

    public bool FavoritesOnly { get; init; }

    public bool IncludeArchived { get; init; }
}
