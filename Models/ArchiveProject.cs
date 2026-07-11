using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectArchive.Models;

public sealed class ArchiveProject
{
    public int Id { get; set; }

    [Required, MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    public ProjectStatus Status { get; set; } = ProjectStatus.Planning;

    public ProjectDifficulty Difficulty { get; set; } = ProjectDifficulty.Medium;

    public ProjectPriority Priority { get; set; } = ProjectPriority.Medium;

    public ProjectCategory Category { get; set; } = ProjectCategory.Other;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public DateTime? FinishedAt { get; set; }

    public bool IsFavorite { get; set; }

    public bool IsArchived { get; set; }

    [MaxLength(500)]
    public string GitHubUrl { get; set; } = string.Empty;

    [MaxLength(500)]
    public string DemoUrl { get; set; } = string.Empty;

    [MaxLength(500)]
    public string FolderPath { get; set; } = string.Empty;

    public double EstimatedHours { get; set; }

    public double ActualHours { get; set; }

    [MaxLength(40)]
    public string Version { get; set; } = "1.0.0";

    [MaxLength(80)]
    public string License { get; set; } = string.Empty;

    public int Rating { get; set; } = 3;

    [MaxLength(4000)]
    public string Notes { get; set; } = string.Empty;

    [MaxLength(500)]
    public string CoverImagePath { get; set; } = string.Empty;

    public ICollection<ProjectTechnology> ProjectTechnologies { get; set; } = new List<ProjectTechnology>();

    public ICollection<ProjectTag> ProjectTags { get; set; } = new List<ProjectTag>();

    public ICollection<ProjectImage> Images { get; set; } = new List<ProjectImage>();

    [NotMapped]
    public string TechnologySummary => string.Join(", ", ProjectTechnologies.Select(item => item.Technology.Name).Order());

    [NotMapped]
    public string TagSummary => string.Join(", ", ProjectTags.Select(item => item.Tag.Name).Order());

    [NotMapped]
    public string FavoriteGlyph => IsFavorite ? "★" : string.Empty;
}
