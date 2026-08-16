using Behrouzan.BuildingBlocks.Core.Results;

namespace Behrouzan.BuildingBlocks.Core.Tests;

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

    [Fact]
    public void Success_WithNullValue_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() =>
            Result<string>.Success(null!));
    }

    [Fact]
    public void Match_WhenResultIsSuccess_ShouldExecuteSuccessFunction()
    {
        var result = Result<string>.Success("Product");

        var output = result.Match(
            onSuccess: value => $"Success: {value}",
            onFailure: errors => "Failed");

        Assert.Equal("Success: Product", output);
    }

    [Fact]
    public void Match_WhenResultIsFailure_ShouldExecuteFailureFunction()
    {
        var error = Error.NotFound(
            "Product.NotFound",
            "Product was not found.");

        var result = Result<string>.Failure(error);

        var output = result.Match(
            onSuccess: value => $"Success: {value}",
            onFailure: errors => $"Failed: {errors[0].Code}");

        Assert.Equal("Failed: Product.NotFound", output);
    }

    [Fact]
    public void Match_WithNullSuccessFunction_ShouldThrow()
    {
        var result = Result<string>.Success("Product");

        Assert.Throws<ArgumentNullException>(() =>
            result.Match<string>(
                null!,
                errors => "Failed"));
    }

    [Fact]
    public void Match_WithNullFailureFunction_ShouldThrow()
    {
        var result = Result<string>.Success("Product");

        Assert.Throws<ArgumentNullException>(() =>
            result.Match<string>(
                value => $"Success: {value}",
                null!));
    }

    [Fact]
    public void Map_WhenResultIsSuccess_ShouldTransformValue()
    {
        var result = Result<string>.Success("Product");

        var mapped = result.Map(
            value => value.Length);

        Assert.True(mapped.IsSuccess);
        Assert.Equal(7, mapped.Value);
        Assert.Empty(mapped.Errors);
    }

    [Fact]
    public void Map_WhenResultIsFailure_ShouldPreserveErrors()
    {
        var error = Error.NotFound(
            "Product.NotFound",
            "Product was not found.");

        var result = Result<string>.Failure(error);

        var mapped = result.Map(
            value => value.Length);

        Assert.True(mapped.IsFailure);
        Assert.Single(mapped.Errors);
        Assert.Equal(error, mapped.FirstError);
    }

    [Fact]
    public void Map_WithNullMapper_ShouldThrow()
    {
        var result = Result<string>.Success("Product");

        Assert.Throws<ArgumentNullException>(() =>
            result.Map<int>(null!));
    }

    [Fact]
    public void Bind_WhenResultIsSuccess_ShouldExecuteBinder()
    {
        var result = Result<string>.Success("Product");

        var bound = result.Bind(
            value => Result<int>.Success(value.Length));

        Assert.True(bound.IsSuccess);
        Assert.Equal(7, bound.Value);
    }

    [Fact]
    public void Bind_WhenResultIsFailure_ShouldPreserveErrors()
    {
        var error = Error.NotFound(
            "Product.NotFound",
            "Product was not found.");

        var result = Result<string>.Failure(error);

        var bound = result.Bind(
            value => Result<int>.Success(value.Length));

        Assert.True(bound.IsFailure);
        Assert.Single(bound.Errors);
        Assert.Equal(error, bound.FirstError);
    }

    [Fact]
    public void Bind_WithNullBinder_ShouldThrow()
    {
        var result = Result<string>.Success("Product");

        Assert.Throws<ArgumentNullException>(() =>
            result.Bind<int>(null!));
    }
}