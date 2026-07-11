using System.IO;
using Microsoft.EntityFrameworkCore;
using ProjectArchive.Models;

namespace ProjectArchive.Data;

public sealed class ArchiveDbContext : DbContext
{
    public ArchiveDbContext(DbContextOptions<ArchiveDbContext> options)
        : base(options)
    {
    }

    public DbSet<ArchiveProject> Projects => Set<ArchiveProject>();

    public DbSet<Technology> Technologies => Set<Technology>();

    public DbSet<Tag> Tags => Set<Tag>();

    public DbSet<ProjectImage> Images => Set<ProjectImage>();

    public DbSet<ProjectTechnology> ProjectTechnologies => Set<ProjectTechnology>();

    public DbSet<ProjectTag> ProjectTags => Set<ProjectTag>();

    public static string DefaultDatabasePath
    {
        get
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var directory = Path.Combine(appData, "ProjectArchive");
            return Path.Combine(directory, "project-archive.db");
        }
    }

    public static ArchiveDbContext Create(string? databasePath = null)
    {
        databasePath ??= DefaultDatabasePath;
        var directory = Path.GetDirectoryName(databasePath);

        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var options = new DbContextOptionsBuilder<ArchiveDbContext>()
            .UseSqlite($"Data Source={databasePath}")
            .Options;

        return new ArchiveDbContext(options);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ArchiveProject>()
            .Property(project => project.Status)
            .HasConversion<string>();

        modelBuilder.Entity<ArchiveProject>()
            .Property(project => project.Difficulty)
            .HasConversion<string>();

        modelBuilder.Entity<ArchiveProject>()
            .Property(project => project.Priority)
            .HasConversion<string>();

        modelBuilder.Entity<ArchiveProject>()
            .Property(project => project.Category)
            .HasConversion<string>();

        modelBuilder.Entity<ProjectTechnology>()
            .HasKey(projectTechnology => new { projectTechnology.ArchiveProjectId, projectTechnology.TechnologyId });

        modelBuilder.Entity<ProjectTechnology>()
            .HasOne(projectTechnology => projectTechnology.ArchiveProject)
            .WithMany(project => project.ProjectTechnologies)
            .HasForeignKey(projectTechnology => projectTechnology.ArchiveProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProjectTechnology>()
            .HasOne(projectTechnology => projectTechnology.Technology)
            .WithMany(technology => technology.ProjectTechnologies)
            .HasForeignKey(projectTechnology => projectTechnology.TechnologyId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProjectTag>()
            .HasKey(projectTag => new { projectTag.ArchiveProjectId, projectTag.TagId });

        modelBuilder.Entity<ProjectTag>()
            .HasOne(projectTag => projectTag.ArchiveProject)
            .WithMany(project => project.ProjectTags)
            .HasForeignKey(projectTag => projectTag.ArchiveProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProjectTag>()
            .HasOne(projectTag => projectTag.Tag)
            .WithMany(tag => tag.ProjectTags)
            .HasForeignKey(projectTag => projectTag.TagId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProjectImage>()
            .HasOne(image => image.ArchiveProject)
            .WithMany(project => project.Images)
            .HasForeignKey(image => image.ArchiveProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Technology>()
            .HasIndex(technology => technology.Name)
            .IsUnique();

        modelBuilder.Entity<Tag>()
            .HasIndex(tag => tag.Name)
            .IsUnique();
    }
}
