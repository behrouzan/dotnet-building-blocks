using Behzad.BuildingBlocks.Core.Results;

namespace Behzad.BuildingBlocks.Core.Tests;

public class ResultTests
{
    [Fact]
    public void Success_ShouldCreateSuccessfulResult()
    {
        var result = Result.Success();

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Empty(result.Errors);
        Assert.Null(result.FirstError);
    }

    [Fact]
    public void Failure_WithSingleError_ShouldCreateFailedResult()
    {
        var error = Error.NotFound(
            "Product.NotFound",
            "Product was not found.");

        var result = Result.Failure(error);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Single(result.Errors);
        Assert.Equal(error, result.FirstError);
    }

    [Fact]
    public void Failure_WithMultipleErrors_ShouldContainAllErrors()
    {
        var error1 = Error.Validation(
            "User.Email.Invalid",
            "Email is invalid.",
            "email");

        var error2 = Error.Validation(
            "User.Password.Invalid",
            "Password is invalid.",
            "password");

        var result = Result.Failure(error1, error2);

        Assert.True(result.IsFailure);
        Assert.Equal(2, result.Errors.Count);
        Assert.Equal(error1, result.Errors[0]);
        Assert.Equal(error2, result.Errors[1]);
    }

    [Fact]
    public void Failure_WithEmptyErrorCollection_ShouldThrow()
    {
        var errors = Array.Empty<Error>();

        Assert.Throws<InvalidOperationException>(() =>
            Result.Failure(errors));
    }

    [Fact]
    public void Failure_WithNullErrorInsideCollection_ShouldThrow()
    {
        var errors = new Error?[]
        {
            Error.Validation(
                "User.Email.Invalid",
                "Email is invalid.",
                "email"),

            null
        };

        Assert.Throws<ArgumentException>(() =>
            Result.Failure(errors!));
    }

    [Fact]
    public void Combine_WhenAllResultsAreSuccessful_ShouldReturnSuccess()
    {
        var result1 = Result.Success();
        var result2 = Result.Success();
        var result3 = Result.Success();

        var combined = Result.Combine(
            result1,
            result2,
            result3);

        Assert.True(combined.IsSuccess);
        Assert.Empty(combined.Errors);
    }

    [Fact]
    public void Combine_WhenSomeResultsFail_ShouldReturnAllErrors()
    {
        var error1 = Error.Validation(
            "User.Email.Invalid",
            "Email is invalid.",
            "email");

        var error2 = Error.Validation(
            "User.Password.TooShort",
            "Password is too short.",
            "password");

        var error3 = Error.Conflict(
            "User.Email.AlreadyExists",
            "Email already exists.",
            "email");

        var result1 = Result.Failure(error1, error2);
        var result2 = Result.Success();
        var result3 = Result.Failure(error3);

        var combined = Result.Combine(
            result1,
            result2,
            result3);

        Assert.True(combined.IsFailure);
        Assert.Equal(3, combined.Errors.Count);

        Assert.Equal(error1, combined.Errors[0]);
        Assert.Equal(error2, combined.Errors[1]);
        Assert.Equal(error3, combined.Errors[2]);
    }

    [Fact]
    public void Combine_WithNoResults_ShouldReturnSuccess()
    {
        var combined = Result.Combine();

        Assert.True(combined.IsSuccess);
        Assert.Empty(combined.Errors);
    }
}