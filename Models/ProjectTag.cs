namespace ProjectArchive.Models;

public sealed class ProjectTag
{
    public int ArchiveProjectId { get; set; }

    public ArchiveProject ArchiveProject { get; set; } = null!;

    public int TagId { get; set; }

    public Tag Tag { get; set; } = null!;
}
