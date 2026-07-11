using System.ComponentModel.DataAnnotations;

namespace ProjectArchive.Models;

public sealed class ProjectImage
{
    public int Id { get; set; }

    public int ArchiveProjectId { get; set; }

    public ArchiveProject ArchiveProject { get; set; } = null!;

    [Required, MaxLength(500)]
    public string Path { get; set; } = string.Empty;

    public int SortOrder { get; set; }
}
