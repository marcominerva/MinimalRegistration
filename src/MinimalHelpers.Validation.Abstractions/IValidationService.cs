namespace MinimalHelpers.Validation;

public interface IValidationService
{
    Task<MinimalValidationResult> ValidateAsync<T>(T input) where T : class;
}