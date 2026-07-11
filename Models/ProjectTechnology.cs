namespace ProjectArchive.Models;

public sealed class ProjectTechnology
{
    public int ArchiveProjectId { get; set; }

    public ArchiveProject ArchiveProject { get; set; } = null!;

    public int TechnologyId { get; set; }

    public Technology Technology { get; set; } = null!;
}
