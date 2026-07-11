namespace ProjectArchive.Services;

public sealed class ProjectStatistics
{
    public int TotalProjects { get; init; }

    public int CompletedProjects { get; init; }

    public int InProgressProjects { get; init; }

    public int ArchivedProjects { get; init; }

    public int FavoriteProjects { get; init; }

    public double TotalActualHours { get; init; }

    public IReadOnlyList<TechnologyUsage> TopTechnologies { get; init; } = [];
}
