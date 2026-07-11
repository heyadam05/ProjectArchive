namespace ProjectArchive.Models;

public enum ProjectStatus
{
    Planning,
    InProgress,
    Completed,
    Paused,
    Archived,
    Cancelled
}

public enum ProjectDifficulty
{
    Easy,
    Medium,
    Hard,
    Expert
}

public enum ProjectPriority
{
    Low,
    Medium,
    High,
    Critical
}

public enum ProjectCategory
{
    Desktop,
    Web,
    Api,
    Game,
    Cli,
    Library,
    Ai,
    Automation,
    Mobile,
    Embedded,
    Other
}
