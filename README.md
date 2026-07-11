# Project Archive

Project Archive is a WPF desktop application for developers who want one local place to track their software projects. It stores project metadata, technologies, tags, notes, links, progress, ratings, favorites, estimated hours, actual hours, and archive state in a local SQLite database.

## Features

- Dark WPF dashboard with sidebar navigation and project statistics.
- SQLite persistence through Entity Framework Core.
- MVVM structure using CommunityToolkit.Mvvm.
- Project CRUD workflow: create, edit, save, select, and delete projects.
- Search by name, description, category, status, technologies, and tags.
- Filters for status, difficulty, category, priority, favorites, and archived projects.
- Dashboard counters for total, completed, in-progress, archived, favorite projects, and actual hours.
- JSON and CSV export to the user's Documents folder.
- Startup seed data so the first run is immediately usable.
- Smoke/integration test runner without external test framework dependencies.

## Tech Stack

- C#
- .NET 10
- WPF
- SQLite
- Entity Framework Core
- CommunityToolkit.Mvvm

## Requirements

- Windows
- .NET SDK 10.0 or newer

Check your SDK:

```powershell
dotnet --info
```

## Run The App

From the solution folder:

```powershell
cd D:\VisualStudioCode\Projects\ProjectArchive
dotnet run --project ProjectArchive\ProjectArchive.csproj
```

The app creates its local database automatically at:

```text
%LOCALAPPDATA%\ProjectArchive\project-archive.db
```

On first launch, it seeds a couple of sample projects.

## Run Tests

The repository includes a lightweight smoke test project that verifies validation, SQLite persistence, filtering, statistics, JSON export, CSV export, and deletion.

```powershell
cd D:\VisualStudioCode\Projects\ProjectArchive
dotnet run --project ProjectArchive.Tests\ProjectArchive.Tests.csproj
```

Expected output:

```text
All Project Archive smoke tests passed.
```

## Build

```powershell
cd D:\VisualStudioCode\Projects\ProjectArchive
dotnet build
```

## Formatting

```powershell
dotnet format
```

## Project Structure

```text
ProjectArchive
├── ProjectArchive
│   ├── Data
│   ├── Models
│   ├── Repositories
│   ├── Services
│   ├── ViewModels
│   ├── App.xaml
│   └── MainWindow.xaml
└── ProjectArchive.Tests
    └── Program.cs
```

## Architecture

The application follows a simple layered design:

```text
WPF View
↓
ViewModel
↓
Services
↓
Repository
↓
Entity Framework Core
↓
SQLite
```

The UI is intentionally thin. Validation, filtering, statistics, persistence, and export behavior live outside the view so the project is easier to maintain and test.

## Export Location

JSON and CSV exports are written to:

```text
%USERPROFILE%\Documents\ProjectArchive\Exports
```

File names include a timestamp, for example:

```text
project-archive-20260709-143000.json
```

## Current Limitations

- PDF export is not implemented yet.
- Image gallery storage exists in the data model, but the UI does not yet include drag-and-drop screenshot management.
- The sidebar is currently visual navigation; the main screen contains the implemented workflow.

