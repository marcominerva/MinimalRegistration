namespace MinimalHelpers.Validation;

public class MinimalValidationResult(bool isValid, IDictionary<string, string[]>? errors = null)
{
    public bool IsValid { get; } = isValid;

    public IDictionary<string, string[]>? Errors { get; } = errors;
}