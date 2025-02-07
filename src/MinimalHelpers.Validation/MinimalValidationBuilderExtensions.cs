using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MinimalHelpers.Validation;

public static class MinimalValidationBuilderExtensions
{
    public static void WithMiniValidator(this IMinimalValidationBuilder builder)
        => builder.Services.TryAddSingleton<IValidationService, MiniValidationService>();
}