using ProjectArchive.Models;

namespace ProjectArchive.Services;

public sealed class ProjectValidator
{
    public ProjectValidationResult Validate(ProjectEditorState editorState)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(editorState.Name))
        {
            errors.Add("Project name is required.");
        }

        ValidateUrl(editorState.GitHubUrl, "GitHub URL", errors);
        ValidateUrl(editorState.DemoUrl, "Demo URL", errors);

        if (editorState.EstimatedHours < 0)
        {
            errors.Add("Estimated hours must be greater than or equal to 0.");
        }

        if (editorState.ActualHours < 0)
        {
            errors.Add("Actual hours must be greater than or equal to 0.");
        }

        if (editorState.Rating is < 1 or > 5)
        {
            errors.Add("Rating must be between 1 and 5.");
        }

        return new ProjectValidationResult(errors);
    }

    private static void ValidateUrl(string value, string fieldName, ICollection<string> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri)
            || uri.Scheme is not ("http" or "https"))
        {
            errors.Add($"{fieldName} must be a valid HTTP or HTTPS URL.");
        }
    }
}
