using Microsoft.Extensions.DependencyInjection;

namespace MinimalHelpers.Validation;

internal class DefaultMinimalValidationBuilder(IServiceCollection services) : IMinimalValidationBuilder
{
    public IServiceCollection Services { get; } = services;
}