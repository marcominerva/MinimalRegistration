using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace MinimalHelpers.Validation;

internal class ValidatorFilter<T>(IValidationService validationService, IOptions<ValidationOptions> options) : IEndpointFilter where T : class
{
    private readonly ValidationOptions validationOptions = options.Value;

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (context.Arguments.FirstOrDefault(a => a?.GetType() == typeof(T)) is not T input)
        {
            return TypedResults.BadRequest();
        }

        var validationResult = await validationService.ValidateAsync(input);
        if (validationResult.IsValid)
        {
            return await next.Invoke(context);
        }

        var errors = validationResult.Errors ?? new Dictionary<string, string[]>();
        var httpContext = context.HttpContext;

        var result = TypedResults.Problem(
            statusCode: StatusCodes.Status400BadRequest,
            instance: httpContext.Request.Path,
            title: validationOptions.ValidationErrorTitleMessageFactory?.Invoke(context, errors) ?? "One or more validation errors occurred",
            extensions: new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier,
                ["errors"] = validationOptions.ErrorResponseFormat == ErrorResponseFormat.Default ? errors : errors.SelectMany(e => e.Value.Select(m => new { Name = e.Key, Message = m })).ToArray()
            }
        );

        return result;
    }
}