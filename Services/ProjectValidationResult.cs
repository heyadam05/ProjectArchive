namespace ProjectArchive.Services;

public sealed class ProjectValidationResult
{
    public ProjectValidationResult(IReadOnlyList<string> errors)
    {
        Errors = errors;
    }

    public bool IsValid => Errors.Count == 0;

    public IReadOnlyList<string> Errors { get; }

    public string Message => string.Join(Environment.NewLine, Errors);
}
