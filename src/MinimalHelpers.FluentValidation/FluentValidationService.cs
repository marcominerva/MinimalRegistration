using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MinimalHelpers.Validation;

namespace MinimalHelpers.FluentValidation;

internal class FluentValidationService(IServiceProvider serviceProvider) : IValidationService
{
    public async Task<MinimalValidationResult> ValidateAsync<T>(T input) where T : class
    {
        using var scope = serviceProvider.CreateScope();
        var validator = scope.ServiceProvider.GetRequiredService<IValidator<T>>();

        var result = await validator.ValidateAsync(input);
        return new MinimalValidationResult(result.IsValid, result.ToDictionary());
    }
}