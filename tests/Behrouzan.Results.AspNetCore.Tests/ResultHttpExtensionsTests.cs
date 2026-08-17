using Behrouzan.Results.AspNetCore;
using Behrouzan.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;


namespace Behrouzan.Results.AspNetCore.Tests;

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
 
        var httpResult = result.ToHttpResult();

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

        Assert.Equal(
            "urn:behrouzan:problem:not-found",
            root.GetProperty("type").GetString());
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

        var httpResult = result.ToHttpResult();

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

        var httpResult = result.ToHttpResult();

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

        var httpResult = result.ToHttpResult();

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

    [Fact]
    public async Task ToHttpResult_ShouldUseConfiguredStatusCodeFromDependencyInjection()
    {
        var result = Result<string>.Failure(
            Error.NotFound(
                "Product.NotFound",
                "Product was not found."));

        var services = new ServiceCollection();

        services.AddLogging();
        services.AddProblemDetails();

        services.AddBehrouzanResultHttp(options =>
        {
            options.MapStatusCode(
                ErrorType.NotFound,
                StatusCodes.Status410Gone);
        });

        var serviceProvider =
            services.BuildServiceProvider();

        var context = new DefaultHttpContext
        {
            RequestServices = serviceProvider
        };

        context.Response.Body = new MemoryStream();

        var httpResult =
            result.ToHttpResult();

        await httpResult.ExecuteAsync(context);

        Assert.Equal(
            StatusCodes.Status410Gone,
            context.Response.StatusCode);
    }

    [Fact]
    public async Task ToHttpResult_ShouldUseConfiguredProblemTypeBaseFromDependencyInjection()
    {
        var result = Result<string>.Failure(
            Error.NotFound(
                "Product.NotFound",
                "Product was not found."));

        var services = new ServiceCollection();

        services.AddLogging();
        services.AddProblemDetails();

        services.AddBehrouzanResultHttp(options =>
        {
            options.ProblemTypeBase =
                "https://api.example.com/problems";
        });

        var provider = services.BuildServiceProvider();

        var context = new DefaultHttpContext
        {
            RequestServices = provider
        };

        context.Response.Body = new MemoryStream();

        var httpResult = result.ToHttpResult();

        await httpResult.ExecuteAsync(context);

        context.Response.Body.Position = 0;

        using var reader =
            new StreamReader(context.Response.Body);

        var body = await reader.ReadToEndAsync();

        using var document =
            JsonDocument.Parse(body);

        Assert.Equal(
            "https://api.example.com/problems/not-found",
            document.RootElement
                .GetProperty("type")
                .GetString());
    }


    [Fact]
    public async Task ToHttpResult_WhenNonGenericResultIsSuccessful_ShouldReturn204()
    {
        var result = Result.Success();

        var context = CreateHttpContext();

        var httpResult = result.ToHttpResult();

        await httpResult.ExecuteAsync(context);

        Assert.Equal(
            StatusCodes.Status204NoContent,
            context.Response.StatusCode);
    }

    [Fact]
    public async Task ToHttpResult_WhenNonGenericResultFails_ShouldReturnProblemDetails()
    {
        var result = Result.Failure(
            Error.NotFound(
                "Product.NotFound",
                "Product was not found."));

        var context = CreateHttpContext();

        var httpResult = result.ToHttpResult();

        await httpResult.ExecuteAsync(context);

        Assert.Equal(
            StatusCodes.Status404NotFound,
            context.Response.StatusCode);

        context.Response.Body.Position = 0;

        using var reader =
            new StreamReader(context.Response.Body);

        var body = await reader.ReadToEndAsync();

        using var document =
            JsonDocument.Parse(body);

        var root = document.RootElement;

        Assert.Equal(
            "urn:behrouzan:problem:not-found",
            root.GetProperty("type").GetString());

        Assert.Equal(
            "Resource not found",
            root.GetProperty("title").GetString());

        Assert.Equal(
            StatusCodes.Status404NotFound,
            root.GetProperty("status").GetInt32());

        var errors = root.GetProperty("errors");

        Assert.Single(errors.EnumerateArray());

        Assert.Equal(
            "Product.NotFound",
            errors[0].GetProperty("code").GetString());
    }

    [Fact]
    public async Task ToHttpResult_WhenConflict_ShouldReturn409()
    {
        var result = Result<string>.Failure(
            Error.Conflict(
                "User.Email.AlreadyExists",
                "Email already exists."));

        var context = CreateHttpContext();

        var httpResult = result.ToHttpResult();

        await httpResult.ExecuteAsync(context);

        Assert.Equal(
            StatusCodes.Status409Conflict,
            context.Response.StatusCode);
    }

    [Fact]
    public async Task ToHttpResult_WhenUnauthorized_ShouldReturn401()
    {
        var result = Result<string>.Failure(
            Error.Unauthorized(
                "Authentication.Required",
                "Authentication is required."));

        var context = CreateHttpContext();

        var httpResult = result.ToHttpResult();

        await httpResult.ExecuteAsync(context);

        Assert.Equal(
            StatusCodes.Status401Unauthorized,
            context.Response.StatusCode);
    }

    [Fact]
    public async Task ToHttpResult_WhenForbidden_ShouldReturn403()
    {
        var result = Result<string>.Failure(
            Error.Forbidden(
                "Access.Forbidden",
                "Access is forbidden."));

        var context = CreateHttpContext();

        var httpResult = result.ToHttpResult();

        await httpResult.ExecuteAsync(context);

        Assert.Equal(
            StatusCodes.Status403Forbidden,
            context.Response.StatusCode);
    }

    [Fact]
    public async Task ToHttpResult_WhenRateLimited_ShouldReturn429()
    {
        var result = Result<string>.Failure(
            Error.RateLimit(
                "Requests.RateLimit",
                "Too many requests."));

        var context = CreateHttpContext();

        var httpResult = result.ToHttpResult();

        await httpResult.ExecuteAsync(context);

        Assert.Equal(
            StatusCodes.Status429TooManyRequests,
            context.Response.StatusCode);
    }

    [Fact]
    public async Task ToHttpResult_WhenUnavailable_ShouldReturn503()
    {
        var result = Result<string>.Failure(
            Error.Unavailable(
                "Service.Unavailable",
                "Service is unavailable."));

        var context = CreateHttpContext();

        var httpResult = result.ToHttpResult();

        await httpResult.ExecuteAsync(context);

        Assert.Equal(
            StatusCodes.Status503ServiceUnavailable,
            context.Response.StatusCode);
    }

    [Fact]
    public async Task ToHttpResult_WhenTimeout_ShouldReturn504()
    {
        var result = Result<string>.Failure(
            Error.Timeout(
                "Operation.Timeout",
                "Operation timed out."));

        var context = CreateHttpContext();

        var httpResult = result.ToHttpResult();

        await httpResult.ExecuteAsync(context);

        Assert.Equal(
            StatusCodes.Status504GatewayTimeout,
            context.Response.StatusCode);
    }

    [Fact]
    public async Task ToHttpResult_WhenErrorsAreMixed_ShouldUseDeterministicPriority()
    {
        var result = Result<string>.Failure(
            Error.Validation(
                "User.Email.Invalid",
                "Email is invalid.",
                "email"),

            Error.Conflict(
                "User.Email.AlreadyExists",
                "Email already exists.",
                "email"));

        var context = CreateHttpContext();

        var httpResult = result.ToHttpResult();

        await httpResult.ExecuteAsync(context);

        Assert.Equal(
            StatusCodes.Status409Conflict,
            context.Response.StatusCode);
    }

    [Fact]
    public async Task ToHttpResult_WhenConfigured_ShouldUseCustomStatusCode()
    {
        var result = Result<string>.Failure(
            Error.NotFound(
                "Product.NotFound",
                "Product was not found."));

        var services = new ServiceCollection();

        services.AddLogging();
        services.AddProblemDetails();

        services.AddBehrouzanResultHttp(options =>
        {
            options.MapStatusCode(
                ErrorType.NotFound,
                StatusCodes.Status410Gone);
        });

        var provider = services.BuildServiceProvider();

        var context = new DefaultHttpContext
        {
            RequestServices = provider
        };

        context.Response.Body = new MemoryStream();

        var httpResult = result.ToHttpResult();

        await httpResult.ExecuteAsync(context);

        Assert.Equal(
            StatusCodes.Status410Gone,
            context.Response.StatusCode);
    }

    [Fact]
    public async Task ToHttpResult_WhenMixedErrorOrderChanges_ShouldKeepSamePriority()
    {
        var result = Result<string>.Failure(
            Error.Conflict(
                "User.Email.AlreadyExists",
                "Email already exists.",
                "email"),

            Error.Validation(
                "User.Email.Invalid",
                "Email is invalid.",
                "email"));

        var context = CreateHttpContext();

        var httpResult = result.ToHttpResult();

        await httpResult.ExecuteAsync(context);

        Assert.Equal(
            StatusCodes.Status409Conflict,
            context.Response.StatusCode);
    }

    [Fact]
    public async Task ToHttpResult_WhenConfigured_ShouldUseCustomProblemTypeBase()
    {
        var result = Result<string>.Failure(
            Error.NotFound(
                "Product.NotFound",
                "Product was not found."));

        var services = new ServiceCollection();

        services.AddLogging();
        services.AddProblemDetails();

        services.AddBehrouzanResultHttp(options =>
        {
            options.ProblemTypeBase =
                "https://api.example.com/problems";
        });

        var provider = services.BuildServiceProvider();

        var context = new DefaultHttpContext
        {
            RequestServices = provider
        };

        context.Response.Body = new MemoryStream();

        var httpResult = result.ToHttpResult();

        await httpResult.ExecuteAsync(context);

        context.Response.Body.Position = 0;

        using var reader =
            new StreamReader(context.Response.Body);

        var body = await reader.ReadToEndAsync();

        using var document =
            JsonDocument.Parse(body);

        Assert.Equal(
            "https://api.example.com/problems/not-found",
            document.RootElement
                .GetProperty("type")
                .GetString());
    }
}