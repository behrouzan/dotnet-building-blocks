using Behrouzan.Results;
using Behrouzan.Results.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace Behrouzan.Results.AspNetCore.Tests;

public class ResultActionExtensionsTests
{
    [Fact]
    public async Task ToActionResult_WhenSuccessful_ShouldReturn200_WithValue()
    {
        var result =
            Result<string>.Success("Laptop");

        var actionResult =
            result.ToActionResult();

        var context =
            CreateActionContext();

        await actionResult.ExecuteResultAsync(context);

        Assert.Equal(
            StatusCodes.Status200OK,
            context.HttpContext.Response.StatusCode);

        context.HttpContext.Response.Body.Position = 0;

        using var reader =
            new StreamReader(
                context.HttpContext.Response.Body);

        var body =
            await reader.ReadToEndAsync();

        Assert.Contains(
            "Laptop",
            body);
    }

    [Fact]
    public async Task ToActionResult_WhenNotFound_ShouldReturn404ProblemDetails()
    {
        var result =
            Result<string>.Failure(
                Error.NotFound(
                    "Product.NotFound",
                    "Product was not found."));

        var actionResult =
            result.ToActionResult();

        var context =
            CreateActionContext(
                "controller-trace-123");

        await actionResult.ExecuteResultAsync(context);

        Assert.Equal(
            StatusCodes.Status404NotFound,
            context.HttpContext.Response.StatusCode);

        context.HttpContext.Response.Body.Position = 0;

        using var reader =
            new StreamReader(
                context.HttpContext.Response.Body);

        var body =
            await reader.ReadToEndAsync();

        using var document =
            JsonDocument.Parse(body);

        var root =
            document.RootElement;

        Assert.Equal(
            "Resource not found",
            root.GetProperty("title").GetString());

        Assert.Equal(
            "Product was not found.",
            root.GetProperty("detail").GetString());

        Assert.Equal(
            "urn:behrouzan:problem:not-found",
            root.GetProperty("type").GetString());

        Assert.Equal(
            "controller-trace-123",
            root.GetProperty("traceId").GetString());

        var errors =
            root.GetProperty("errors");

        Assert.Single(
            errors.EnumerateArray());

        Assert.Equal(
            "Product.NotFound",
            errors[0]
                .GetProperty("code")
                .GetString());
    }

    [Fact]
    public async Task ToActionResult_WhenNonGenericSuccess_ShouldReturn204()
    {
        var result =
            Result.Success();

        var actionResult =
            result.ToActionResult();

        var context =
            CreateActionContext();

        await actionResult.ExecuteResultAsync(context);

        Assert.Equal(
            StatusCodes.Status204NoContent,
            context.HttpContext.Response.StatusCode);
    }

    private static ActionContext CreateActionContext(
        string? traceId = null)
    {
        var services =
            new ServiceCollection();

        services.AddLogging();
        services.AddControllers();
        services.AddBehrouzanResultHttp();

        var provider =
            services.BuildServiceProvider();

        var httpContext =
            new DefaultHttpContext
            {
                RequestServices = provider
            };

        httpContext.Response.Body =
            new MemoryStream();

        if (!string.IsNullOrWhiteSpace(traceId))
        {
            httpContext.TraceIdentifier =
                traceId;
        }

        return new ActionContext
        {
            HttpContext = httpContext
        };
    }

    [Fact]
    public async Task ToActionResult_WhenValidationFails_ShouldReturn400()
    {
        var result =
            Result<string>.Failure(
                Error.Validation(
                    "User.Email.Invalid",
                    "Email is invalid.",
                    "email"));

        var actionResult =
            result.ToActionResult();

        var context =
            CreateActionContext();

        await actionResult.ExecuteResultAsync(context);

        Assert.Equal(
            StatusCodes.Status400BadRequest,
            context.HttpContext.Response.StatusCode);

        context.HttpContext.Response.Body.Position = 0;

        using var reader =
            new StreamReader(
                context.HttpContext.Response.Body);

        var body =
            await reader.ReadToEndAsync();

        using var document =
            JsonDocument.Parse(body);

        var root =
            document.RootElement;

        Assert.Equal(
            "Validation failed",
            root.GetProperty("title").GetString());

        var error =
            root.GetProperty("errors")[0];

        Assert.Equal(
            "User.Email.Invalid",
            error.GetProperty("code").GetString());

        Assert.Equal(
            "email",
            error.GetProperty("propertyPath").GetString());

        Assert.Equal(
            "Validation",
            error.GetProperty("type").GetString());
    }

    [Fact]
    public async Task ToActionResult_WhenConflict_ShouldReturn409()
    {
        var result =
            Result<string>.Failure(
                Error.Conflict(
                    "Product.OutOfStock",
                    "Product is out of stock."));

        var actionResult =
            result.ToActionResult();

        var context =
            CreateActionContext();

        await actionResult.ExecuteResultAsync(context);

        Assert.Equal(
            StatusCodes.Status409Conflict,
            context.HttpContext.Response.StatusCode);
    }

    [Fact]
    public async Task ToActionResult_ShouldUseConfiguredStatusCode()
    {
        var result =
            Result<string>.Failure(
                Error.NotFound(
                    "Product.NotFound",
                    "Product was not found."));

        var services =
            new ServiceCollection();

        services.AddLogging();
        services.AddControllers();

        services.AddBehrouzanResultHttp(options =>
        {
            options.MapStatusCode(
                ErrorType.NotFound,
                StatusCodes.Status410Gone);
        });

        var provider =
            services.BuildServiceProvider();

        var httpContext =
            new DefaultHttpContext
            {
                RequestServices = provider
            };

        httpContext.Response.Body =
            new MemoryStream();

        var context =
            new ActionContext
            {
                HttpContext = httpContext
            };

        var actionResult =
            result.ToActionResult();

        await actionResult.ExecuteResultAsync(context);

        Assert.Equal(
            StatusCodes.Status410Gone,
            context.HttpContext.Response.StatusCode);
    }
}