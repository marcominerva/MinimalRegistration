using MiniValidation;

namespace MinimalHelpers.Validation;

internal class MiniValidationService : IValidationService
{
    public async Task<MinimalValidationResult> ValidateAsync<T>(T input) where T : class
    {
        var (isValid, errors) = await MiniValidator.TryValidateAsync(input);
        return new MinimalValidationResult(isValid, errors);
    }
}