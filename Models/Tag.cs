using System.ComponentModel.DataAnnotations;

namespace ProjectArchive.Models;

public sealed class Tag
{
    public int Id { get; set; }

    [Required, MaxLength(60)]
    public string Name { get; set; } = string.Empty;

    public ICollection<ProjectTag> ProjectTags { get; set; } = new List<ProjectTag>();
}
