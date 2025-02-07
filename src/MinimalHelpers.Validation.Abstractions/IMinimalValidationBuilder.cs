using Microsoft.Extensions.DependencyInjection;

namespace MinimalHelpers.Validation;

public interface IMinimalValidationBuilder
{
    IServiceCollection Services { get; }
}