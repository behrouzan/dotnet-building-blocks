using Behrouzan.BuildingBlocks.Core.Results;
using Behrouzan.BuildingBlocks.AspNetCore.Results;
using Microsoft.AspNetCore.Http;
namespace Behrouzan.BuildingBlocks.AspNetCore.Tests;

public class ResultHttpMappingTests
{
    [Fact]
    public void NotFoundError_ShouldMapTo404()
    {
        var result = Result<string>.Failure(
            Error.NotFound(
                "Product.NotFound",
                "Product was not found."));

        var statusCode = ResultHttpMapper.GetStatusCode(result.Errors);

        Assert.Equal(404, statusCode);
    }

    [Fact]
    public void ValidationError_ShouldMapTo400()
    {
        var result = Result<string>.Failure(
            Error.Validation(
                "Product.Id.Invalid",
                "Product id is invalid.",
                "id"));

        var statusCode = ResultHttpMapper.GetStatusCode(result.Errors);

        Assert.Equal(400, statusCode);
    }

    [Fact]
    public void UnauthorizedError_ShouldMapTo401()
    {
        var result = Result<string>.Failure(
            Error.Unauthorized(
                "Authentication.Required",
                "Authentication is required."));

        var statusCode = ResultHttpMapper.GetStatusCode(result.Errors);

        Assert.Equal(401, statusCode);
    }

    [Fact]
    public void ForbiddenError_ShouldMapTo403()
    {
        var result = Result<string>.Failure(
            Error.Forbidden(
                "Access.Forbidden",
                "Access is forbidden."));

        var statusCode = ResultHttpMapper.GetStatusCode(result.Errors);

        Assert.Equal(403, statusCode);
    }

    [Fact]
    public void ConflictError_ShouldMapTo409()
    {
        var result = Result<string>.Failure(
            Error.Conflict(
                "User.Email.AlreadyExists",
                "Email already exists."));

        var statusCode = ResultHttpMapper.GetStatusCode(result.Errors);

        Assert.Equal(409, statusCode);
    }

    [Fact]
    public void RateLimitError_ShouldMapTo429()
    {
        var result = Result<string>.Failure(
            Error.RateLimit(
                "Requests.RateLimit",
                "Too many requests."));

        var statusCode = ResultHttpMapper.GetStatusCode(result.Errors);

        Assert.Equal(429, statusCode);
    }

    [Fact]
    public void UnavailableError_ShouldMapTo503()
    {
        var result = Result<string>.Failure(
            Error.Unavailable(
                "Service.Unavailable",
                "The service is unavailable."));

        var statusCode = ResultHttpMapper.GetStatusCode(result.Errors);

        Assert.Equal(503, statusCode);
    }

    [Fact]
    public void TimeoutError_ShouldMapTo504()
    {
        var result = Result<string>.Failure(
            Error.Timeout(
                "Operation.Timeout",
                "The operation timed out."));

        var statusCode = ResultHttpMapper.GetStatusCode(result.Errors);

        Assert.Equal(504, statusCode);
    }

    [Fact]
    public void MixedErrors_ShouldUseDeterministicPriority()
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

        var statusCode = ResultHttpMapper.GetStatusCode(result.Errors);

        Assert.Equal(409, statusCode);
    }

    [Fact]
    public void MixedErrors_ShouldNotDependOnErrorOrder()
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

        var statusCode = ResultHttpMapper.GetStatusCode(result.Errors);

        Assert.Equal(409, statusCode);
    }

    [Fact]
    public void NotFoundError_ShouldHaveNotFoundTitle()
    {
        var errors = new[]
        {
            Error.NotFound(
                "Product.NotFound",
                "Product was not found.")
        };

        var title = ResultHttpMapper.GetTitle(errors);

        Assert.Equal(
            "Resource not found",
            title);
    }


    [Fact]
    public void GeneralFailure_ShouldMapTo500()
    {
        var result = Result<string>.Failure(
            Error.Failure(
                "General.Failure",
                "The operation failed."));

        var statusCode =
            ResultHttpMapper.GetStatusCode(result.Errors);

        Assert.Equal(
            StatusCodes.Status500InternalServerError,
            statusCode);
    }

    [Fact]
    public void GeneralFailure_ShouldHaveGenericTitle()
    {
        var errors = new[]
        {
            Error.Failure(
                "General.Failure",
                "The operation failed.")
        };

        var title =
            ResultHttpMapper.GetTitle(errors);

        Assert.Equal(
            "Request failed",
            title);
    }


    [Fact]
    public void NotFoundError_ShouldHaveProblemType()
    {
        var errors = new[]
        {
            Error.NotFound(
                "Product.NotFound",
                "Product was not found.")
        };

        var problemType =
            ResultHttpMapper.GetProblemType(errors);

        Assert.Equal(
            "urn:behrouzan:problem:not-found",
            problemType);
    }

    [Fact]
    public void GetStatusCode_ShouldUseConfiguredStatusCode()
    {
        var errors = new[]
        {
            Error.NotFound(
                "Product.NotFound",
                "Product was not found.")
        };

        var options = new ResultHttpOptions();

        options.MapStatusCode(
            ErrorType.NotFound,
            StatusCodes.Status410Gone);

        var statusCode =
            ResultHttpMapper.GetStatusCode(
                errors,
                options);

        Assert.Equal(
            StatusCodes.Status410Gone,
            statusCode);
    }
}