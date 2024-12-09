# Minimal APIs Helpers

[![Lint Code Base](https://github.com/marcominerva/MinimalHelpers/actions/workflows/linter.yml/badge.svg)](https://github.com/marcominerva/MinimalHelpers/actions/workflows/linter.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/marcominerva/MinimalHelpers/blob/master/LICENSE)

A collection of helpers libraries for Minimal API projects.

## MinimalHelpers.Routing

[![Nuget](https://img.shields.io/nuget/v/MinimalHelpers.Routing)](https://www.nuget.org/packages/MinimalHelpers.Routing)
[![Nuget](https://img.shields.io/nuget/dt/MinimalHelpers.Routing)](https://www.nuget.org/packages/MinimalHelpers.Routing)

A library that provides Routing helpers for Minimal API projects for automatic endpoints registration using Reflection.

### Installation

The library is available on [NuGet](https://www.nuget.org/packages/MinimalHelpers.Routing). Just search for *MinimalHelpers.Routing* in the **Package Manager GUI** or run the following command in the **.NET CLI**:

```shell
dotnet add package MinimalHelpers.Routing
```

### Usage

Create a class to hold your route handlers registration and make it implementing the `IEndpointRouteHandlerBuilder` interface:

```csharp
public class PeopleEndpoints : MinimalHelpers.Routing.IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/people", GetList);
        endpoints.MapGet("/api/people/{id:guid}", Get);
        endpoints.MapPost("/api/people", Insert);
        endpoints.MapPut("/api/people/{id:guid}", Update);
        endpoints.MapDelete("/api/people/{id:guid}", Delete);
    }

    // ...
}
```

Call the `MapEndpoints()` extension method on the **WebApplication** object inside *Program.cs* before the `Run()` method invocation:

```csharp
// using MinimalHelpers.Routing;
app.MapEndpoints();

app.Run();
```

By default, `MapEndpoints()` will scan the calling Assembly to search for classes that implement the `IEndpointRouteHandlerBuilder` interface. If your route handlers are defined in another Assembly, you have two alternatives:

- Use the `MapEndpoints()` overload that takes the Assembly to scan as argument
- Use the `MapEndpointsFromAssemblyContaining<T>()` extension method and specify a type that is contained in the Assembly you want to scan

You can also explicitly decide what types (among the ones that implement the `IRouteEndpointHandlerBuilder` interface) you want to actually map, passing a predicate to the `MapEndpoints` method:

```csharp
app.MapEndpoints(type =>
{
    if (type.Name.StartsWith("Products"))
    {
        return false;
    }

    return true;
});
```

> **Note**
These methods rely on Reflection to scan the Assembly and find the classes that implement the `IEndpointRouteHandlerBuilder` interface. This can have a performance impact, especially in large projects. If you have performance issues, consider using the explicit registration method. Moreover, this solution is incompatibile with Native AOT.

## MinimalHelpers.Routing.Analyzers

[![Nuget](https://img.shields.io/nuget/v/MinimalHelpers.Routing.Analyzers)](https://www.nuget.org/packages/MinimalHelpers.Routing.Analyzers)
[![Nuget](https://img.shields.io/nuget/dt/MinimalHelpers.Routing.Analyzers)](https://www.nuget.org/packages/MinimalHelpers.Routing.Analyzers)

A library that provides a Source Generator for automatic endpoints registration in Minimal API projects.

### Installation

The library is available on [NuGet](https://www.nuget.org/packages/MinimalHelpers.Routing.Analyzers). Just search for *MinimalHelpers.Routing* in the **Package Manager GUI** or run the following command in the **.NET CLI**:

```shell
dotnet add package MinimalHelpers.Routing.Analyzers
```

### Usage

Create a class to hold your route handlers registration and make it implementing the `IEndpointRouteHandlerBuilder` interface:

```csharp
public class PeopleEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/people", GetList);
        endpoints.MapGet("/api/people/{id:guid}", Get);
        endpoints.MapPost("/api/people", Insert);
        endpoints.MapPut("/api/people/{id:guid}", Update);
        endpoints.MapDelete("/api/people/{id:guid}", Delete);
    }

    // ...
}
```

> **Note**
You only need to use the **MinimalHelpers.Routing.Analyzers** package. With this Source Generator, the `IEndpointRouteHandlerBuilder` interface is auto-generated.

Call the `MapEndpoints()` extension method on the **WebApplication** object inside *Program.cs* before the `Run()` method invocation:

```csharp
app.MapEndpoints();

app.Run();
```

> **Note**
The `MapEndpoints` method is generated by the Source Generator.

## MinimalHelpers.OpenApi

[![Nuget](https://img.shields.io/nuget/v/MinimalHelpers.OpenApi)](https://www.nuget.org/packages/MinimalHelpers.OpenApi)
[![Nuget](https://img.shields.io/nuget/dt/MinimalHelpers.OpenApi)](https://www.nuget.org/packages/MinimalHelpers.OpenApi)

A library that provides OpenApi helpers for Minimal API projects.

### Installation

The library is available on [NuGet](https://www.nuget.org/packages/MinimalHelpers.OpenApi). Just search for *MinimalHelpers.OpenApi* in the **Package Manager GUI** or run the following command in the **.NET CLI**:

```shell
dotnet add package MinimalHelpers.OpenApi
```

### Usage

***Extension methods for OpenApi***

This library provides some extensions methods that simplify the OpenAPI configuration in Minimal API projects. For example, it is possible to customize the description of a response using its status code:

```csharp
endpoints.MapPost("login", LoginAsync)
    .AllowAnonymous()
    .WithValidation<LoginRequest>()
    .Produces<LoginResponse>(StatusCodes.Status200OK)
    .Produces<LoginResponse>(StatusCodes.Status206PartialContent)
    .Produces(StatusCodes.Status403Forbidden)
    .ProducesValidationProblem()
    .WithOpenApi(operation =>
    {
        operation.Summary = "Performs the login of a user";

        operation.Response(StatusCodes.Status200OK).Description = "Login successful";
        operation.Response(StatusCodes.Status206PartialContent).Description = "The user is logged in, but the password has expired and must be changed";
        operation.Response(StatusCodes.Status400BadRequest).Description = "Incorrect username and/or password";
        operation.Response(StatusCodes.Status403Forbidden).Description = "The user was blocked due to too many failed logins";

        return operation;
    });
 ```

 ***Extension methods for RouteHandlerBuilder***

 Often we have endpoints with multiple 4xx return values, each of which produces a `ProblemDetails` response:

 ```csharp
 endpoints.MapGet("/api/people/{id:guid}", Get)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status401Unauthorized)
    .ProducesProblem(StatusCodes.Status403Forbidden)
    .ProducesProblem(StatusCodes.Status404NotFound);
 ```

 To avoid multiple calls to `ProducesProblem`, we can use the `ProducesDefaultProblem` extension method provided by the library:

 ```csharp
endpoints.MapGet("/api/people/{id:guid}", Get)
    .ProducesDefaultProblem(StatusCodes.Status400BadRequest, StatusCodes.Status401Unauthorized,
        StatusCodes.Status403Forbidden, StatusCodes.Status404NotFound);
 ```

## MinimalHelpers.Validation

[![Nuget](https://img.shields.io/nuget/v/MinimalHelpers.Validation)](https://www.nuget.org/packages/MinimalHelpers.Validation)
[![Nuget](https://img.shields.io/nuget/dt/MinimalHelpers.Validation)](https://www.nuget.org/packages/MinimalHelpers.Validation)

A library that provides an Endpoint filter for Minimal API projects to perform validation with Data Annotations, using the <a href="https://github.com/DamianEdwards/MiniValidation">MiniValidation</a> library.

### Installation

The library is available on [NuGet](https://www.nuget.org/packages/MinimalHelpers.Validation). Just search for *MinimalHelpers.Validation* in the **Package Manager GUI** or run the following command in the **.NET CLI**:

```shell
dotnet add package MinimalHelpers.Validation
```

### Usage

Decorates a class with attributes to define the validation rules:

```csharp
using System.ComponentModel.DataAnnotations;

public class Person
{
    [Required]
    [MaxLength(20)]
    public string? FirstName { get; set; }

    [Required]
    [MaxLength(20)]
    public string? LastName { get; set; }

    [MaxLength(50)]
    public string? City { get; set; }
}
```

Add the `WithValidation<T>()` extension method to enable the validation filter:

```csharp
using MinimalHelpers.Validation;

app.MapPost("/api/people", (Person person) =>
    {
        // ...
    })
    .WithValidation<Person>();
```

If the validation fails, the response will be a `400 Bad Request` with a `ValidationProblemDetails` object containing the validation errors, for example:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred",
  "status": 400,
  "instance": "/api/people",
  "traceId": "00-009c0162ba678cae2ee391815dbbb59d-0a3a5b0c16d053e6-00",
  "errors": {
    "FirstName": [
      "The field FirstName must be a string or array type with a maximum length of '20'."
    ],
    "LastName": [
      "The LastName field is required."
    ]
  }
}
```

If you want to customize validation, you can use the `ConfigureValidation` extension method:

```csharp
using MinimalHelpers.Validation;

builder.Services.ConfigureValidation(options =>
{
    // If you want to get errors as a list instead of a dictionary.
    options.ErrorResponseFormat = ErrorResponseFormat.List;

    // The default is "One or more validation errors occurred"
    options.ValidationErrorTitleMessageFactory =
        (context, errors) => $"There was {errors.Values.Sum(v => v.Length)} error(s)";
});
```

You can use the `ValidationErrorTitleMessageFactory`, for example, if you want to localized the `title` property of the response using a RESX file.

## MinimalHelpers.FluentValidation

[![Nuget](https://img.shields.io/nuget/v/MinimalHelpers.FluentValidation)](https://www.nuget.org/packages/MinimalHelpers.FluentValidation)
[![Nuget](https://img.shields.io/nuget/dt/MinimalHelpers.FluentValidation)](https://www.nuget.org/packages/MinimalHelpers.FluentValidation)

A library that provides an Endpoint filter for Minimal API projects to perform validation using <a href="https://fluentvalidation.net">FluentValidation</a>.

### Installation

The library is available on [NuGet](https://www.nuget.org/packages/MinimalHelpers.FluentValidation). Just search for *MinimalHelpers.FluentValidation* in the **Package Manager GUI** or run the following command in the **.NET CLI**:

```shell
dotnet add package MinimalHelpers.FluentValidation
```

### Usage

Create a class that extends AbstractValidator<T> and define the validation rules:

```csharp
using FluentValidation;

public record class Product(string Name, string Description, double UnitPrice);

public class ProductValidator : AbstractValidator<Product>
{
    public ProductValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MaximumLength(50).EmailAddress();
        RuleFor(p => p.Description).MaximumLength(500);
        RuleFor(p => p.UnitPrice).GreaterThan(0);
    }
}
```

Register validators in the Service Collection:

```csharp
using FluentValidation;

// Assuming the validators are in the same assembly as the Program class
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

```

Add the `WithValidation<T>()` extension method to enable the validation filter:

```csharp
using MinimalHelpers.FluentValidation;

app.MapPost("/api/products", (Product product) =>
    {
        // ...
    })
    .WithValidation<Product>();
```

If the validation fails, the response will be a `400 Bad Request` with a `ValidationProblemDetails` object containing the validation errors, for example:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred",
  "status": 400,
  "instance": "/api/products",
  "traceId": "00-f4ced0ae470424dd04cbcebe5f232dc5-bbdcc59f310ebfb8-00",
  "errors": {
    "Name": [
      "'Name' cannot be empty."
    ],
    "UnitPrice": [
      "'Unit Price' must be grater than '0'."
    ]
  }
}
```

If you want to customize validation, you can use the `ConfigureValidation` extension method:

```csharp
using MinimalHelpers.Validation;

builder.Services.ConfigureValidation(options =>
{
    // If you want to get errors as a list instead of a dictionary.
    options.ErrorResponseFormat = ErrorResponseFormat.List;

    // The default is "One or more validation errors occurred"
    options.ValidationErrorTitleMessageFactory =
        (context, errors) => $"There was {errors.Values.Sum(v => v.Length)} error(s)";
});
```

You can use the `ValidationErrorTitleMessageFactory`, for example, if you want to localized the `title` property of the response using a RESX file.


**Contribute**

The project is constantly evolving. Contributions are welcome. Feel free to file issues and pull requests on the repo and we'll address them as we can. 
