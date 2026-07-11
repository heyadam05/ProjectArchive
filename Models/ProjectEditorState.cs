namespace ProjectArchive.Models;

public sealed class ProjectEditorState
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public ProjectStatus Status { get; set; } = ProjectStatus.Planning;

    public ProjectDifficulty Difficulty { get; set; } = ProjectDifficulty.Medium;

    public ProjectPriority Priority { get; set; } = ProjectPriority.Medium;

    public ProjectCategory Category { get; set; } = ProjectCategory.Other;

    public bool IsFavorite { get; set; }

    public bool IsArchived { get; set; }

    public string GitHubUrl { get; set; } = string.Empty;

    public string DemoUrl { get; set; } = string.Empty;

    public string FolderPath { get; set; } = string.Empty;

    public double EstimatedHours { get; set; }

    public double ActualHours { get; set; }

    public string Version { get; set; } = "1.0.0";

    public string License { get; set; } = string.Empty;

    public int Rating { get; set; } = 3;

    public string Notes { get; set; } = string.Empty;

    public string CoverImagePath { get; set; } = string.Empty;

    public string TechnologyNames { get; set; } = string.Empty;

    public string TagNames { get; set; } = string.Empty;

    public static ProjectEditorState FromProject(ArchiveProject project)
    {
        return new ProjectEditorState
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            Status = project.Status,
            Difficulty = project.Difficulty,
            Priority = project.Priority,
            Category = project.Category,
            IsFavorite = project.IsFavorite,
            IsArchived = project.IsArchived,
            GitHubUrl = project.GitHubUrl,
            DemoUrl = project.DemoUrl,
            FolderPath = project.FolderPath,
            EstimatedHours = project.EstimatedHours,
            ActualHours = project.ActualHours,
            Version = project.Version,
            License = project.License,
            Rating = project.Rating,
            Notes = project.Notes,
            CoverImagePath = project.CoverImagePath,
            TechnologyNames = string.Join(", ", project.ProjectTechnologies.Select(item => item.Technology.Name).Order()),
            TagNames = string.Join(", ", project.ProjectTags.Select(item => item.Tag.Name).Order())
        };
    }
}
