using Microsoft.Extensions.DependencyInjection;

namespace MinimalHelpers.Validation;

/// <summary>
/// Provides extension methods for adding validation support.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the validation services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configureOptions">The <see cref="Action{ValidationOptions}"/> to configure the validation options.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IMinimalValidationBuilder ConfigureValidation(this IServiceCollection services, Action<ValidationOptions> configureOptions)
    {
        services.Configure(configureOptions);
        return new DefaultMinimalValidationBuilder(services);
    }
}
