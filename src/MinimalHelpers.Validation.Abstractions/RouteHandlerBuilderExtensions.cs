using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace MinimalHelpers.Validation;

/// <summary>
/// Extension methods for <see cref="RouteHandlerBuilder"/> to add validation.
/// </summary>
/// <seealso cref="RouteHandlerBuilder"/>
public static class RouteHandlerBuilderExtensions
{
    /// <summary>
    /// Registers a <seealso cref="ValidatorFilter{T}"/> of type <typeparamref name="T"/> onto the route handler to validate the <typeparamref name="T"/> object.
    /// </summary>
    /// <typeparam name="T">The type of the object to validate.</typeparam>
    /// <param name="builder">The <see cref="RouteHandlerBuilder"/> to add validation filter to.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the route handler.</returns>
    /// <remarks>The validation is performed with Data annotations, using <a href="https://github.com/DamianEdwards/MiniValidation">MiniValidation</a>.</remarks>
    /// <seealso cref="ValidatorFilter{T}"/>
    public static RouteHandlerBuilder WithValidation<T>(this RouteHandlerBuilder builder) where T : class
    {
        builder.AddEndpointFilter<ValidatorFilter<T>>()
            .ProducesValidationProblem();

        return builder;
    }
}