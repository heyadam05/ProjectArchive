using Microsoft.EntityFrameworkCore;
using ProjectArchive.Data;
using ProjectArchive.Models;

namespace ProjectArchive.Repositories;

public sealed class ProjectRepository : IProjectRepository
{
    private readonly ArchiveDbContext context;

    public ProjectRepository(ArchiveDbContext context)
    {
        this.context = context;
    }

    public async Task<IReadOnlyList<ArchiveProject>> GetAllAsync()
    {
        return await ProjectQuery()
            .OrderByDescending(project => project.IsFavorite)
            .ThenByDescending(project => project.UpdatedAt)
            .ToListAsync();
    }

    public async Task<ArchiveProject?> GetByIdAsync(int id)
    {
        return await ProjectQuery().FirstOrDefaultAsync(project => project.Id == id);
    }

    public async Task<ArchiveProject> SaveAsync(ProjectEditorState editorState)
    {
        var project = editorState.Id == 0
            ? new ArchiveProject { CreatedAt = DateTime.Now }
            : await context.Projects
                .Include(item => item.ProjectTechnologies)
                .Include(item => item.ProjectTags)
                .FirstAsync(item => item.Id == editorState.Id);

        if (project.Id == 0)
        {
            context.Projects.Add(project);
        }

        ApplyEditorState(project, editorState);
        await ReplaceTechnologiesAsync(project, editorState.TechnologyNames);
        await ReplaceTagsAsync(project, editorState.TagNames);

        await context.SaveChangesAsync();
        return (await GetByIdAsync(project.Id))!;
    }

    public async Task DeleteAsync(int id)
    {
        var project = await context.Projects.FindAsync(id);

        if (project is null)
        {
            return;
        }

        context.Projects.Remove(project);
        await context.SaveChangesAsync();
    }

    private IQueryable<ArchiveProject> ProjectQuery()
    {
        return context.Projects
            .AsNoTracking()
            .Include(project => project.ProjectTechnologies)
            .ThenInclude(item => item.Technology)
            .Include(project => project.ProjectTags)
            .ThenInclude(item => item.Tag)
            .Include(project => project.Images);
    }

    private static void ApplyEditorState(ArchiveProject project, ProjectEditorState editorState)
    {
        project.Name = editorState.Name.Trim();
        project.Description = editorState.Description.Trim();
        project.Status = editorState.Status;
        project.Difficulty = editorState.Difficulty;
        project.Priority = editorState.Priority;
        project.Category = editorState.Category;
        project.IsFavorite = editorState.IsFavorite;
        project.IsArchived = editorState.IsArchived || editorState.Status == ProjectStatus.Archived;
        project.GitHubUrl = editorState.GitHubUrl.Trim();
        project.DemoUrl = editorState.DemoUrl.Trim();
        project.FolderPath = editorState.FolderPath.Trim();
        project.EstimatedHours = editorState.EstimatedHours;
        project.ActualHours = editorState.ActualHours;
        project.Version = editorState.Version.Trim();
        project.License = editorState.License.Trim();
        project.Rating = editorState.Rating;
        project.Notes = editorState.Notes.Trim();
        project.CoverImagePath = editorState.CoverImagePath.Trim();
        project.UpdatedAt = DateTime.Now;
        project.FinishedAt = editorState.Status == ProjectStatus.Completed ? DateTime.Now : project.FinishedAt;
    }

    private async Task ReplaceTechnologiesAsync(ArchiveProject project, string technologyNames)
    {
        project.ProjectTechnologies.Clear();

        foreach (var name in SplitNames(technologyNames))
        {
            var technology = await context.Technologies.FirstOrDefaultAsync(item => item.Name == name)
                ?? context.Technologies.Add(new Technology { Name = name, Type = GuessTechnologyType(name) }).Entity;

            project.ProjectTechnologies.Add(new ProjectTechnology
            {
                ArchiveProject = project,
                Technology = technology
            });
        }
    }

    private async Task ReplaceTagsAsync(ArchiveProject project, string tagNames)
    {
        project.ProjectTags.Clear();

        foreach (var name in SplitNames(tagNames))
        {
            var tag = await context.Tags.FirstOrDefaultAsync(item => item.Name == name)
                ?? context.Tags.Add(new Tag { Name = name }).Entity;

            project.ProjectTags.Add(new ProjectTag
            {
                ArchiveProject = project,
                Tag = tag
            });
        }
    }

    private static IReadOnlyList<string> SplitNames(string value)
    {
        return value
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static string GuessTechnologyType(string name)
    {
        var lowerName = name.ToLowerInvariant();

        if (lowerName is "sqlite" or "mysql" or "postgresql" or "mongodb")
        {
            return "Database";
        }

        if (lowerName is "wpf" or "react" or "spring boot" or ".net")
        {
            return "Framework";
        }

        return "Language";
    }
}
