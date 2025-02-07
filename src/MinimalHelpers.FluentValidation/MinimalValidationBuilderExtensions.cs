using Microsoft.Extensions.DependencyInjection.Extensions;
using MinimalHelpers.Validation;

namespace MinimalHelpers.FluentValidation;

public static class MinimalValidationBuilderExtensions
{
    public static void WithFluentValidation(this IMinimalValidationBuilder builder)
        => builder.Services.TryAddSingleton<IValidationService, FluentValidationService>();
}