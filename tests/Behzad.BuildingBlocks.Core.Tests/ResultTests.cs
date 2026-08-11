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
}