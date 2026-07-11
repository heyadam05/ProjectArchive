using System.ComponentModel.DataAnnotations;

namespace ProjectArchive.Models;

public sealed class Technology
{
    public int Id { get; set; }

    [Required, MaxLength(80)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(40)]
    public string Type { get; set; } = "Technology";

    public ICollection<ProjectTechnology> ProjectTechnologies { get; set; } = new List<ProjectTechnology>();
}
