# Project Archive

Project Archive is a Windows WPF desktop application for developers who want one local place to track software projects. It stores project metadata, technologies, tags, notes, links, progress, ratings, favorites, estimated hours, actual hours, and archive state in SQLite.

## Highlights

- Use a dark WPF dashboard with sidebar navigation and project statistics.
- Store data locally with SQLite and Entity Framework Core.
- Follow an MVVM structure with CommunityToolkit.Mvvm.
- Create, edit, save, select, and delete projects.
- Search by name, description, category, status, technologies, and tags.
- Filter by status, difficulty, category, priority, favorites, and archived state.
- Track totals, completed projects, in-progress projects, archived projects, favorites, and actual hours.
- Export project data to JSON or CSV in the user's Documents folder.
- Seed sample data on first launch.
- Run smoke tests without an external test framework.

## Requirements

- Windows
- .NET SDK 10.0 or newer

Check your SDK:

```powershell
dotnet --info
```

## Run

From the solution folder:

```powershell
cd D:\VisualStudioCode\Projects\ProjectArchive
dotnet run --project ProjectArchive\ProjectArchive.csproj
```

On first launch, the app creates a local database and seeds a few sample projects.

## Test

The smoke test project verifies validation, SQLite persistence, filtering, statistics, JSON export, CSV export, and deletion.

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

## Format

```powershell
dotnet format
```

## Data And Files

The app stores its database at:

```text
%LOCALAPPDATA%\ProjectArchive\project-archive.db
```

JSON and CSV exports are written to:

```text
%USERPROFILE%\Documents\ProjectArchive\Exports
```

Export file names include a timestamp, for example:

```text
project-archive-20260709-143000.json
```
