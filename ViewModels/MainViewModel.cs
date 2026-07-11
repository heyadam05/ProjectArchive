using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectArchive.Data;
using ProjectArchive.Models;
using ProjectArchive.Services;

namespace ProjectArchive.ViewModels;

public sealed class MainViewModel : ObservableObject
{
    private readonly ProjectService projectService;
    private readonly StatisticsService statisticsService;
    private readonly ExportService exportService;
    private readonly IThemeService themeService;
    private readonly ISettingsStorageService settingsStorageService;
    private readonly List<ArchiveProject> allProjects = [];
    private ObservableCollection<ArchiveProject> projects = [];
    private ObservableCollection<ArchiveProject> recentProjects = [];
    private ObservableCollection<ArchiveProject> favoriteProjects = [];
    private ObservableCollection<TechnologyUsage> technologyRows = [];
    private ArchiveProject? selectedProject;
    private ProjectEditorState editor = new();
    private ProjectStatistics statistics = new();
    private string searchText = string.Empty;
    private ProjectStatus? selectedStatusFilter;
    private ProjectDifficulty? selectedDifficultyFilter;
    private ProjectCategory? selectedCategoryFilter;
    private ProjectPriority? selectedPriorityFilter;
    private bool favoritesOnly;
    private bool includeArchived = true;
    private string activeSection = "Dashboard";
    private string pageTitle = "Dashboard";
    private string pageSubtitle = "Track project status, technologies, favorites, notes, and exports.";
    private bool isDarkTheme = true;
    private bool autoSaveEnabled = true;
    private bool dailyBackupsEnabled;
    private string selectedAccentColor = "#3B82F6";
    private bool pendingIsDarkTheme = true;
    private bool pendingAutoSaveEnabled = true;
    private bool pendingDailyBackupsEnabled;
    private string pendingSelectedAccentColor = "#3B82F6";

    public MainViewModel(
        ProjectService projectService,
        StatisticsService statisticsService,
        ExportService exportService,
        IThemeService? themeService = null,
        ISettingsStorageService? settingsStorageService = null)
    {
        this.projectService = projectService;
        this.statisticsService = statisticsService;
        this.exportService = exportService;
        this.themeService = themeService ?? new NullThemeService();
        this.settingsStorageService = settingsStorageService ?? new MemorySettingsStorageService();

        ApplyLoadedSettings(this.settingsStorageService.Load());

        NewProjectCommand = new RelayCommand(StartNewProject);
        SaveProjectCommand = new AsyncRelayCommand(SaveProjectAsync);
        DeleteProjectCommand = new AsyncRelayCommand(DeleteProjectAsync, () => SelectedProject is not null);
        ExportJsonCommand = new AsyncRelayCommand(ExportJsonAsync, () => Projects.Count > 0);
        ExportCsvCommand = new AsyncRelayCommand(ExportCsvAsync, () => Projects.Count > 0);
        ClearFiltersCommand = new RelayCommand(ClearFilters);
        NavigateCommand = new RelayCommand<string>(Navigate);
        SaveSettingsCommand = new RelayCommand(SaveSettings);
        this.themeService.ApplyTheme(IsDarkTheme, SelectedAccentColor);
    }

    public ObservableCollection<ArchiveProject> Projects
    {
        get => projects;
        private set
        {
            if (SetProperty(ref projects, value))
            {
                ExportJsonCommand.NotifyCanExecuteChanged();
                ExportCsvCommand.NotifyCanExecuteChanged();
            }
        }
    }

    public ObservableCollection<ArchiveProject> RecentProjects
    {
        get => recentProjects;
        private set => SetProperty(ref recentProjects, value);
    }

    public ObservableCollection<ArchiveProject> FavoriteProjects
    {
        get => favoriteProjects;
        private set => SetProperty(ref favoriteProjects, value);
    }

    public ObservableCollection<TechnologyUsage> TechnologyRows
    {
        get => technologyRows;
        private set => SetProperty(ref technologyRows, value);
    }

    public ArchiveProject? SelectedProject
    {
        get => selectedProject;
        set
        {
            if (!SetProperty(ref selectedProject, value))
            {
                return;
            }

            if (value is not null)
            {
                Editor = ProjectEditorState.FromProject(value);
            }

            DeleteProjectCommand.NotifyCanExecuteChanged();
        }
    }

    public ProjectEditorState Editor
    {
        get => editor;
        private set => SetProperty(ref editor, value);
    }

    public ProjectStatistics Statistics
    {
        get => statistics;
        private set => SetProperty(ref statistics, value);
    }

    public string SearchText
    {
        get => searchText;
        set
        {
            if (SetProperty(ref searchText, value))
            {
                RefreshProjects();
            }
        }
    }

    public ProjectStatus? SelectedStatusFilter
    {
        get => selectedStatusFilter;
        set
        {
            if (SetProperty(ref selectedStatusFilter, value))
            {
                RefreshProjects();
            }
        }
    }

    public ProjectDifficulty? SelectedDifficultyFilter
    {
        get => selectedDifficultyFilter;
        set
        {
            if (SetProperty(ref selectedDifficultyFilter, value))
            {
                RefreshProjects();
            }
        }
    }

    public ProjectCategory? SelectedCategoryFilter
    {
        get => selectedCategoryFilter;
        set
        {
            if (SetProperty(ref selectedCategoryFilter, value))
            {
                RefreshProjects();
            }
        }
    }

    public ProjectPriority? SelectedPriorityFilter
    {
        get => selectedPriorityFilter;
        set
        {
            if (SetProperty(ref selectedPriorityFilter, value))
            {
                RefreshProjects();
            }
        }
    }

    public bool FavoritesOnly
    {
        get => favoritesOnly;
        set
        {
            if (SetProperty(ref favoritesOnly, value))
            {
                RefreshProjects();
            }
        }
    }

    public bool IncludeArchived
    {
        get => includeArchived;
        set
        {
            if (SetProperty(ref includeArchived, value))
            {
                RefreshProjects();
            }
        }
    }

    public string ActiveSection
    {
        get => activeSection;
        private set
        {
            if (SetProperty(ref activeSection, value))
            {
                NotifyActiveSectionChanged();
            }
        }
    }

    public string PageTitle
    {
        get => pageTitle;
        private set => SetProperty(ref pageTitle, value);
    }

    public string PageSubtitle
    {
        get => pageSubtitle;
        private set => SetProperty(ref pageSubtitle, value);
    }

    public bool IsDarkTheme
    {
        get => isDarkTheme;
        private set => SetProperty(ref isDarkTheme, value);
    }

    public bool AutoSaveEnabled
    {
        get => autoSaveEnabled;
        private set => SetProperty(ref autoSaveEnabled, value);
    }

    public bool DailyBackupsEnabled
    {
        get => dailyBackupsEnabled;
        private set => SetProperty(ref dailyBackupsEnabled, value);
    }

    public string SelectedAccentColor
    {
        get => selectedAccentColor;
        private set => SetProperty(ref selectedAccentColor, value);
    }

    public bool PendingIsDarkTheme
    {
        get => pendingIsDarkTheme;
        set => SetProperty(ref pendingIsDarkTheme, value);
    }

    public bool PendingAutoSaveEnabled
    {
        get => pendingAutoSaveEnabled;
        set => SetProperty(ref pendingAutoSaveEnabled, value);
    }

    public bool PendingDailyBackupsEnabled
    {
        get => pendingDailyBackupsEnabled;
        set => SetProperty(ref pendingDailyBackupsEnabled, value);
    }

    public string PendingSelectedAccentColor
    {
        get => pendingSelectedAccentColor;
        set => SetProperty(ref pendingSelectedAccentColor, value);
    }

    public bool IsDashboardActive => ActiveSection == "Dashboard";

    public bool IsProjectsActive => ActiveSection == "Projects";

    public bool IsTechnologiesActive => ActiveSection == "Technologies";

    public bool IsStatisticsActive => ActiveSection == "Statistics";

    public bool IsFavoritesActive => ActiveSection == "Favorites";

    public bool IsSettingsActive => ActiveSection == "Settings";

    public string DatabasePath => ArchiveDbContext.DefaultDatabasePath;

    public string ExportDirectory => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "ProjectArchive",
        "Exports");

    public IReadOnlyList<string> AccentColorOptions { get; } =
    [
        "#3B82F6",
        "#22C55E",
        "#F59E0B",
        "#EF4444"
    ];

    public IReadOnlyList<ProjectStatus> StatusOptions { get; } = Enum.GetValues<ProjectStatus>();

    public IReadOnlyList<ProjectDifficulty> DifficultyOptions { get; } = Enum.GetValues<ProjectDifficulty>();

    public IReadOnlyList<ProjectCategory> CategoryOptions { get; } = Enum.GetValues<ProjectCategory>();

    public IReadOnlyList<ProjectPriority> PriorityOptions { get; } = Enum.GetValues<ProjectPriority>();

    public IRelayCommand NewProjectCommand { get; }

    public IAsyncRelayCommand SaveProjectCommand { get; }

    public IAsyncRelayCommand DeleteProjectCommand { get; }

    public IAsyncRelayCommand ExportJsonCommand { get; }

    public IAsyncRelayCommand ExportCsvCommand { get; }

    public IRelayCommand ClearFiltersCommand { get; }

    public IRelayCommand<string> NavigateCommand { get; }

    public IRelayCommand SaveSettingsCommand { get; }

    public async Task LoadAsync()
    {
        allProjects.Clear();
        allProjects.AddRange(await projectService.GetAllAsync());
        RefreshProjects();

        if (Projects.Count > 0)
        {
            SelectedProject = Projects[0];
        }
        else
        {
            StartNewProject();
        }
    }

    private async Task SaveProjectAsync()
    {
        try
        {
            var savedProject = await projectService.SaveAsync(Editor);
            await LoadAsync();
            SelectedProject = Projects.FirstOrDefault(project => project.Id == savedProject.Id);
        }
        catch (Exception exception)
        {
            PageSubtitle = exception.Message;
        }
    }

    private async Task DeleteProjectAsync()
    {
        if (SelectedProject is null)
        {
            return;
        }

        await projectService.DeleteAsync(SelectedProject.Id);
        await LoadAsync();
    }

    private async Task ExportJsonAsync()
    {
        var path = CreateExportPath("json");
        await exportService.ExportJsonAsync(allProjects, path);
        PageSubtitle = $"Exported JSON to {path}.";
    }

    private async Task ExportCsvAsync()
    {
        var path = CreateExportPath("csv");
        await exportService.ExportCsvAsync(allProjects, path);
        PageSubtitle = $"Exported CSV to {path}.";
    }

    private void StartNewProject()
    {
        SelectedProject = null;
        Editor = new ProjectEditorState();
        Navigate("Projects");
    }

    private void ClearFilters()
    {
        SearchText = string.Empty;
        SelectedStatusFilter = null;
        SelectedDifficultyFilter = null;
        SelectedCategoryFilter = null;
        SelectedPriorityFilter = null;
        FavoritesOnly = false;
        IncludeArchived = true;
        RefreshProjects();
    }

    private void SaveSettings()
    {
        IsDarkTheme = PendingIsDarkTheme;
        AutoSaveEnabled = PendingAutoSaveEnabled;
        DailyBackupsEnabled = PendingDailyBackupsEnabled;
        SelectedAccentColor = PendingSelectedAccentColor;

        settingsStorageService.Save(new AppSettings
        {
            IsDarkTheme = IsDarkTheme,
            AutoSaveEnabled = AutoSaveEnabled,
            DailyBackupsEnabled = DailyBackupsEnabled,
            AccentColor = SelectedAccentColor
        });

        themeService.ApplyTheme(IsDarkTheme, SelectedAccentColor);
        PageSubtitle = "Settings saved and applied.";
    }

    private void ApplyLoadedSettings(AppSettings settings)
    {
        isDarkTheme = settings.IsDarkTheme;
        autoSaveEnabled = settings.AutoSaveEnabled;
        dailyBackupsEnabled = settings.DailyBackupsEnabled;
        selectedAccentColor = settings.AccentColor;
        pendingIsDarkTheme = settings.IsDarkTheme;
        pendingAutoSaveEnabled = settings.AutoSaveEnabled;
        pendingDailyBackupsEnabled = settings.DailyBackupsEnabled;
        pendingSelectedAccentColor = settings.AccentColor;
    }

    private void Navigate(string? section)
    {
        if (string.IsNullOrWhiteSpace(section))
        {
            return;
        }

        ActiveSection = section;
        PageTitle = section;
        PageSubtitle = section switch
        {
            "Projects" => "Browse, search, filter, and edit your saved projects.",
            "Technologies" => "Review which languages, frameworks, and databases appear across your archive.",
            "Statistics" => "Understand completion, priority, difficulty, and time across all projects.",
            "Favorites" => "Pinned projects you want to keep close at hand.",
            "Settings" => "Local app paths and behavior settings.",
            _ => "Track project status, technologies, favorites, notes, and exports."
        };

        RefreshProjects();
    }

    private void RefreshProjects()
    {
        var filteredProjects = projectService.ApplyFilter(
            allProjects,
            new ProjectFilter
            {
                SearchText = SearchText,
                Status = SelectedStatusFilter,
                Difficulty = SelectedDifficultyFilter,
                Category = SelectedCategoryFilter,
                Priority = SelectedPriorityFilter,
                FavoritesOnly = FavoritesOnly,
                IncludeArchived = IncludeArchived
            });

        Projects = new ObservableCollection<ArchiveProject>(filteredProjects);
        Statistics = statisticsService.Calculate(allProjects);
        RecentProjects = new ObservableCollection<ArchiveProject>(
            allProjects
                .OrderByDescending(project => project.UpdatedAt)
                .Take(5));
        FavoriteProjects = new ObservableCollection<ArchiveProject>(
            allProjects
                .Where(project => project.IsFavorite)
                .OrderByDescending(project => project.UpdatedAt));
        TechnologyRows = new ObservableCollection<TechnologyUsage>(
            allProjects
                .SelectMany(project => project.ProjectTechnologies.Select(item => item.Technology.Name))
                .GroupBy(name => name)
                .Select(group => new TechnologyUsage { Name = group.Key, Count = group.Count() })
                .OrderByDescending(item => item.Count)
                .ThenBy(item => item.Name));

        if (SelectedProject is not null && Projects.All(project => project.Id != SelectedProject.Id))
        {
            SelectedProject = Projects.FirstOrDefault();
        }
    }

    private static string CreateExportPath(string extension)
    {
        var directory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "ProjectArchive",
            "Exports");

        Directory.CreateDirectory(directory);

        return Path.Combine(directory, $"project-archive-{DateTime.Now:yyyyMMdd-HHmmss}.{extension}");
    }

    private void NotifyActiveSectionChanged()
    {
        OnPropertyChanged(nameof(IsDashboardActive));
        OnPropertyChanged(nameof(IsProjectsActive));
        OnPropertyChanged(nameof(IsTechnologiesActive));
        OnPropertyChanged(nameof(IsStatisticsActive));
        OnPropertyChanged(nameof(IsFavoritesActive));
        OnPropertyChanged(nameof(IsSettingsActive));
    }
}
