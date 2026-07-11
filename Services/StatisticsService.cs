using ProjectArchive.Models;

namespace ProjectArchive.Services;

public sealed class StatisticsService
{
    public ProjectStatistics Calculate(IEnumerable<ArchiveProject> projects)
    {
        var projectList = projects.ToList();

        return new ProjectStatistics
        {
            TotalProjects = projectList.Count,
            CompletedProjects = projectList.Count(project => project.Status == ProjectStatus.Completed),
            InProgressProjects = projectList.Count(project => project.Status == ProjectStatus.InProgress),
            ArchivedProjects = projectList.Count(project => project.IsArchived),
            FavoriteProjects = projectList.Count(project => project.IsFavorite),
            TotalActualHours = projectList.Sum(project => project.ActualHours),
            TopTechnologies = projectList
                .SelectMany(project => project.ProjectTechnologies.Select(item => item.Technology.Name))
                .GroupBy(name => name)
                .Select(group => new TechnologyUsage { Name = group.Key, Count = group.Count() })
                .OrderByDescending(item => item.Count)
                .ThenBy(item => item.Name)
                .Take(8)
                .ToList()
        };
    }
}
