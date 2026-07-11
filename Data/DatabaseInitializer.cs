using ProjectArchive.Models;
using ProjectArchive.Repositories;

namespace ProjectArchive.Data;

public sealed class DatabaseInitializer
{
    private readonly ArchiveDbContext context;
    private readonly IProjectRepository projectRepository;

    public DatabaseInitializer(ArchiveDbContext context, IProjectRepository projectRepository)
    {
        this.context = context;
        this.projectRepository = projectRepository;
    }

    public async Task InitializeAsync()
    {
        await context.Database.EnsureCreatedAsync();

        if (context.Projects.Any())
        {
            return;
        }

        var sampleProjects = new[]
        {
            new ProjectEditorState
            {
                Name = "Expense Tracker",
                Description = "Desktop finance tracker with SQLite persistence and WPF UI.",
                Status = ProjectStatus.Completed,
                Difficulty = ProjectDifficulty.Medium,
                Priority = ProjectPriority.High,
                Category = ProjectCategory.Desktop,
                TechnologyNames = "C#, WPF, SQLite",
                TagNames = "Portfolio, Desktop, Database",
                GitHubUrl = "https://github.com/example/expense-tracker",
                EstimatedHours = 30,
                ActualHours = 34,
                Rating = 5,
                IsFavorite = true,
                Notes = "Strong portfolio project focused on data entry, reporting, and persistence."
            },
            new ProjectEditorState
            {
                Name = "Portfolio Website",
                Description = "Personal web portfolio with project case studies.",
                Status = ProjectStatus.InProgress,
                Difficulty = ProjectDifficulty.Easy,
                Priority = ProjectPriority.Medium,
                Category = ProjectCategory.Web,
                TechnologyNames = "HTML, CSS, JavaScript",
                TagNames = "Frontend, Portfolio",
                DemoUrl = "https://example.com",
                EstimatedHours = 18,
                ActualHours = 11,
                Rating = 4
            }
        };

        foreach (var sampleProject in sampleProjects)
        {
            await projectRepository.SaveAsync(sampleProject);
        }
    }
}
