using Behzad.BuildingBlocks.AspNetCore.Results;
using Behzad.BuildingBlocks.Core.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;


namespace Behzad.BuildingBlocks.AspNetCore.Tests;

public class ResultHttpExtensionsTests
{
    [Fact]
    public async Task ToHttpResult_WhenNotFound_ShouldReturn404()
    {
        var result = Result<string>.Failure(
            Error.NotFound(
                "Product.NotFound",
                "Product was not found."));

        var context = CreateHttpContext();
 
        var httpResult = result.ToHttpResult(context);

        await httpResult.ExecuteAsync(context);

        Assert.Equal(
            StatusCodes.Status404NotFound,
            context.Response.StatusCode);

            context.Response.Body.Position = 0;

        using var reader = new StreamReader(context.Response.Body);

        var body = await reader.ReadToEndAsync();

        using var document = JsonDocument.Parse(body);

        var root = document.RootElement;

        Assert.Equal(
            StatusCodes.Status404NotFound,
            root.GetProperty("status").GetInt32());

        Assert.Equal(
            "Resource not found",
            root.GetProperty("title").GetString());

        Assert.Equal(
            "Product was not found.",
            root.GetProperty("detail").GetString());

        
        var errors = root.GetProperty("errors");

        Assert.Equal(
            JsonValueKind.Array,
            errors.ValueKind);

        Assert.Single(
            errors.EnumerateArray());

        var error = errors[0];

        Assert.Equal(
            "Product.NotFound",
            error.GetProperty("code").GetString());

        Assert.Equal(
            "Product was not found.",
            error.GetProperty("message").GetString());

        Assert.Equal(
            JsonValueKind.Null,
            error.GetProperty("propertyPath").ValueKind);

        Assert.Equal(
            "NotFound",
            error.GetProperty("type").GetString());

        Assert.Equal(
            "Error",
            error.GetProperty("severity").GetString());
    }

    [Fact]
    public async Task ToHttpResult_WhenValidationFails_ShouldReturn400_WithFieldErrors()
    {
        var result = Result<string>.Failure(
            Error.Validation(
                "User.Email.Invalid",
                "Email is invalid.",
                "email"),

            Error.Validation(
                "User.Password.TooShort",
                "Password is too short.",
                "password"));

        var context = CreateHttpContext();

        var httpResult = result.ToHttpResult(context);

        await httpResult.ExecuteAsync(context);

        Assert.Equal(
            StatusCodes.Status400BadRequest,
            context.Response.StatusCode);

        context.Response.Body.Position = 0;

        using var reader = new StreamReader(context.Response.Body);
        var body = await reader.ReadToEndAsync();

        using var document = JsonDocument.Parse(body);
        var root = document.RootElement;

        Assert.Equal(
            StatusCodes.Status400BadRequest,
            root.GetProperty("status").GetInt32());

        var errors = root.GetProperty("errors");

        Assert.Equal(2, errors.GetArrayLength());

        Assert.Equal(
            "User.Email.Invalid",
            errors[0].GetProperty("code").GetString());

        Assert.Equal(
            "email",
            errors[0].GetProperty("propertyPath").GetString());

        Assert.Equal(
            "Validation",
            errors[0].GetProperty("type").GetString());

        Assert.Equal(
            "User.Password.TooShort",
            errors[1].GetProperty("code").GetString());

        Assert.Equal(
            "password",
            errors[1].GetProperty("propertyPath").GetString());
    }

    [Fact]
    public async Task ToHttpResult_WhenFailed_ShouldIncludeTraceId()
    {
        var result = Result<string>.Failure(
            Error.NotFound(
                "Product.NotFound",
                "Product was not found."));

        var context = CreateHttpContext("test-trace-123");

        var httpResult = result.ToHttpResult(context);

        await httpResult.ExecuteAsync(context);

        context.Response.Body.Position = 0;

        using var reader =
            new StreamReader(context.Response.Body);

        var body = await reader.ReadToEndAsync();

        using var document =
            JsonDocument.Parse(body);

        var root = document.RootElement;

        Assert.Equal(
            "test-trace-123",
            root.GetProperty("traceId").GetString());
    }



    [Fact]
    public async Task ToHttpResult_WhenSuccessful_ShouldReturn200_WithValue()
    {
        var product = new
        {
            Id = 1,
            Name = "Laptop",
            Price = 1500
        };

        var result = Result<object>.Success(product);

        var context = CreateHttpContext();

        var httpResult = result.ToHttpResult(context);

        await httpResult.ExecuteAsync(context);

        Assert.Equal(
            StatusCodes.Status200OK,
            context.Response.StatusCode);

        context.Response.Body.Position = 0;

        using var reader =
            new StreamReader(context.Response.Body);

        var body = await reader.ReadToEndAsync();

        using var document =
            JsonDocument.Parse(body);

        var root = document.RootElement;

        Assert.Equal(
            1,
            root.GetProperty("id").GetInt32());

        Assert.Equal(
            "Laptop",
            root.GetProperty("name").GetString());

        Assert.Equal(
            1500,
            root.GetProperty("price").GetInt32());
    }


    private static DefaultHttpContext CreateHttpContext(
        string? traceId = null)
    {
        var services = new ServiceCollection();

        services.AddLogging();
        services.AddProblemDetails();

        var serviceProvider = services.BuildServiceProvider();

        var context = new DefaultHttpContext
        {
            RequestServices = serviceProvider
        };

        if (!string.IsNullOrWhiteSpace(traceId))
        {
            context.TraceIdentifier = traceId;
        }

        context.Response.Body = new MemoryStream();

        return context;
    }


}