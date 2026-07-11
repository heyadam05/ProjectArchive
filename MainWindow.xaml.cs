using System.Windows;
using ProjectArchive.Data;
using ProjectArchive.Repositories;
using ProjectArchive.Services;
using ProjectArchive.ViewModels;

namespace ProjectArchive;

public partial class MainWindow : Window
{
    private readonly ArchiveDbContext dbContext;
    private readonly MainViewModel viewModel;

    public MainWindow()
    {
        InitializeComponent();

        dbContext = ArchiveDbContext.Create();
        var repository = new ProjectRepository(dbContext);
        var validator = new ProjectValidator();
        var projectService = new ProjectService(repository, validator);
        var statisticsService = new StatisticsService();
        var exportService = new ExportService();
        var themeService = new AppThemeService();
        var settingsStorageService = new JsonSettingsStorageService();

        viewModel = new MainViewModel(projectService, statisticsService, exportService, themeService, settingsStorageService);
        DataContext = viewModel;

        Loaded += OnLoaded;
        Closed += OnClosed;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        var initializer = new DatabaseInitializer(dbContext, new ProjectRepository(dbContext));
        await initializer.InitializeAsync();
        await viewModel.LoadAsync();
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        dbContext.Dispose();
    }
}
