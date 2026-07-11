using ProjectArchive.Models;

namespace ProjectArchive.Repositories;

public interface IProjectRepository
{
    Task<IReadOnlyList<ArchiveProject>> GetAllAsync();

    Task<ArchiveProject?> GetByIdAsync(int id);

    Task<ArchiveProject> SaveAsync(ProjectEditorState editorState);

    Task DeleteAsync(int id);
}
