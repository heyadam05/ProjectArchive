using ProjectArchive.Models;
using ProjectArchive.Repositories;

namespace ProjectArchive.Services;

public sealed class ProjectService
{
    private readonly IProjectRepository repository;
    private readonly ProjectValidator validator;

    public ProjectService(IProjectRepository repository, ProjectValidator validator)
    {
        this.repository = repository;
        this.validator = validator;
    }

    public Task<IReadOnlyList<ArchiveProject>> GetAllAsync()
    {
        return repository.GetAllAsync();
    }

    public IReadOnlyList<ArchiveProject> ApplyFilter(IEnumerable<ArchiveProject> projects, ProjectFilter filter)
    {
        var query = projects.AsEnumerable();

        if (!filter.IncludeArchived)
        {
            query = query.Where(project => !project.IsArchived);
        }

        if (filter.FavoritesOnly)
        {
            query = query.Where(project => project.IsFavorite);
        }

        if (filter.Status is not null)
        {
            query = query.Where(project => project.Status == filter.Status);
        }

        if (filter.Difficulty is not null)
        {
            query = query.Where(project => project.Difficulty == filter.Difficulty);
        }

        if (filter.Category is not null)
        {
            query = query.Where(project => project.Category == filter.Category);
        }

        if (filter.Priority is not null)
        {
            query = query.Where(project => project.Priority == filter.Priority);
        }

        if (!string.IsNullOrWhiteSpace(filter.SearchText))
        {
            query = query.Where(project => MatchesSearch(project, filter.SearchText));
        }

        return query
            .OrderByDescending(project => project.IsFavorite)
            .ThenByDescending(project => project.UpdatedAt)
            .ToList();
    }

    public async Task<ArchiveProject> SaveAsync(ProjectEditorState editorState)
    {
        var validationResult = validator.Validate(editorState);

        if (!validationResult.IsValid)
        {
            throw new InvalidOperationException(validationResult.Message);
        }

        return await repository.SaveAsync(editorState);
    }

    public Task DeleteAsync(int id)
    {
        return repository.DeleteAsync(id);
    }

    private static bool MatchesSearch(ArchiveProject project, string searchText)
    {
        return SearchableText(project)
            .Contains(searchText.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    private static string SearchableText(ArchiveProject project)
    {
        var technologies = project.ProjectTechnologies.Select(item => item.Technology.Name);
        var tags = project.ProjectTags.Select(item => item.Tag.Name);

        return string.Join(
            ' ',
            new[] { project.Name, project.Description, project.Category.ToString(), project.Status.ToString() }
                .Concat(technologies)
                .Concat(tags));
    }
}
