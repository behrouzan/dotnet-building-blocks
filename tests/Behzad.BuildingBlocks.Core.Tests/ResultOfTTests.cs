using Behzad.BuildingBlocks.Core.Results;

namespace Behzad.BuildingBlocks.Core.Tests;

public class ResultOfTTests
{
    [Fact]
    public void Success_ShouldCreateSuccessfulResult_WithValue()
    {
        const string expectedValue = "Product";

        var result = Result<string>.Success(expectedValue);

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(expectedValue, result.Value);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Failure_ShouldCreateFailedResult_WithError()
    {
        var error = Error.NotFound(
            "Product.NotFound",
            "Product was not found.");

        var result = Result<string>.Failure(error);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Single(result.Errors);
        Assert.Equal(error, result.FirstError);
    }

    [Fact]
    public void Value_ShouldThrow_WhenResultIsFailure()
    {
        var result = Result<string>.Failure(
            Error.NotFound(
                "Product.NotFound",
                "Product was not found."));

        Assert.Throws<InvalidOperationException>(() =>
        {
            _ = result.Value;
        });
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

        var result = Result<string>.Failure(error1, error2);

        Assert.Equal(2, result.Errors.Count);
        Assert.Equal(error1, result.Errors[0]);
        Assert.Equal(error2, result.Errors[1]);
    }
}