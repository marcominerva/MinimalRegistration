using FluentValidation;
using MinimalHelpers.FluentValidation;
using MinimalHelpers.Validation;
using TinyHelpers.AspNetCore.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(options =>
{
    options.AddDefaultResponse();
});

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.ConfigureValidation(options =>
{
    //options.ErrorResponseFormat = ErrorResponseFormat.List;

    // The default is "One or more validation errors occurred"
    //options.ValidationErrorTitleMessageFactory = (context, errors) => $"There was {errors.Values.Sum(v => v.Length)} error(s)";
})
//.WithMiniValidator();
.WithFluentValidation();

builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", builder.Environment.ApplicationName);
    });
}

// Maps all the endpoints.
app.MapEndpoints();

app.Run();