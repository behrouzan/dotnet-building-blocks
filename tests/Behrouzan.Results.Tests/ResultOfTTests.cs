using Behrouzan.Results;

namespace Behrouzan.Results.Tests;

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

    [Fact]
    public void Bind_WhenBinderReturnsNull_ShouldThrow()
    {
        var result = Result<string>.Success("Product");

        Assert.Throws<InvalidOperationException>(() =>
            result.Bind<int>(_ => null!));
    }

    [Fact]
    public async Task MapAsync_ShouldMapValue_WhenResultIsSuccess()
    {
        var result = Result<int>.Success(10);

        var mapped = await result.MapAsync(
            value => Task.FromResult(value * 2));

        Assert.True(mapped.IsSuccess);
        Assert.Equal(20, mapped.Value);
    }

    [Fact]
    public async Task MapAsync_ShouldPreserveErrors_WhenResultIsFailure()
    {
        var error = Error.Failure(
            "Test.Error",
            "Something failed.");

        var result = Result<int>.Failure(error);

        var mapperCalled = false;

        var mapped = await result.MapAsync(value =>
        {
            mapperCalled = true;
            return Task.FromResult(value * 2);
        });

        Assert.True(mapped.IsFailure);
        Assert.False(mapperCalled);
        Assert.Single(mapped.Errors);
        Assert.Equal(error, mapped.Errors[0]);
    }

    [Fact]
    public async Task MapAsync_ShouldThrow_WhenMapperIsNull()
    {
        var result = Result<int>.Success(10);

        Func<int, Task<int>> mapper = null!;

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => result.MapAsync(mapper));
    }

    [Fact]
    public async Task BindAsync_ShouldReturnBoundResult_WhenResultIsSuccess()
    {
        var result = Result<int>.Success(10);

        var bound = await result.BindAsync(
            value => Task.FromResult(
                Result<string>.Success(
                    value.ToString())));

        Assert.True(bound.IsSuccess);
        Assert.Equal("10", bound.Value);
    }

    [Fact]
    public async Task BindAsync_ShouldPreserveErrors_WhenResultIsFailure()
    {
        var error = Error.Failure(
            "Test.Error",
            "Something failed.");

        var result = Result<int>.Failure(error);

        var binderCalled = false;

        var bound = await result.BindAsync(value =>
        {
            binderCalled = true;

            return Task.FromResult(
                Result<string>.Success(
                    value.ToString()));
        });

        Assert.True(bound.IsFailure);
        Assert.False(binderCalled);
        Assert.Single(bound.Errors);
        Assert.Equal(error, bound.Errors[0]);
    }

    [Fact]
    public async Task BindAsync_ShouldThrow_WhenBinderIsNull()
    {
        var result = Result<int>.Success(10);

        Func<int, Task<Result<string>>> binder = null!;

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => result.BindAsync(binder));
    }

    [Fact]
    public async Task BindAsync_ShouldThrow_WhenBinderReturnsNull()
    {
        var result = Result<int>.Success(10);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => result.BindAsync<string>(
                _ => Task.FromResult<Result<string>>(null!)));
    }
    [Fact]
    public void Ensure_ShouldReturnSameResult_WhenPredicateIsTrue()
    {
        var result =
            Result<int>.Success(10);

        var ensured =
            result.Ensure(
                value => value > 0,
                Error.Validation(
                    "Value.Invalid",
                    "Value must be greater than zero."));

        Assert.Same(result, ensured);
        Assert.True(ensured.IsSuccess);
        Assert.Equal(10, ensured.Value);
    }

    [Fact]
    public void Ensure_ShouldReturnFailure_WhenPredicateIsFalse()
    {
        var error =
            Error.Validation(
                "Value.Invalid",
                "Value must be greater than zero.");

        var result =
            Result<int>.Success(0);

        var ensured =
            result.Ensure(
                value => value > 0,
                error);

        Assert.True(ensured.IsFailure);
        Assert.Single(ensured.Errors);
        Assert.Equal(error, ensured.Errors[0]);
    }

    [Fact]
    public void Ensure_ShouldPreserveExistingFailure()
    {
        var originalError =
            Error.NotFound(
                "Product.NotFound",
                "Product was not found.");

        var result =
            Result<int>.Failure(originalError);

        var predicateCalled = false;

        var ensured =
            result.Ensure(
                value =>
                {
                    predicateCalled = true;
                    return value > 0;
                },
                Error.Validation(
                    "Value.Invalid",
                    "Value is invalid."));

        Assert.False(predicateCalled);
        Assert.Same(result, ensured);
        Assert.Equal(originalError, ensured.Errors[0]);
    }

    [Fact]
    public void Ensure_ShouldThrow_WhenPredicateIsNull()
    {
        var result =
            Result<int>.Success(10);

        Func<int, bool> predicate = null!;

        Assert.Throws<ArgumentNullException>(
            () => result.Ensure(
                predicate,
                Error.Failure(
                    "Test.Error",
                    "Something failed.")));
    }

    [Fact]
    public void Ensure_ShouldThrow_WhenErrorIsNull()
    {
        var result =
            Result<int>.Success(10);

        Assert.Throws<ArgumentNullException>(
            () => result.Ensure(
                value => value > 0,
                null!));
    }

    [Fact]
    public void Tap_ShouldExecuteAction_WhenResultIsSuccess()
    {
        var result = Result<int>.Success(10);
        var capturedValue = 0;

        var tapped = result.Tap(
            value => capturedValue = value);

        Assert.Equal(10, capturedValue);
        Assert.Same(result, tapped);
    }

    [Fact]
    public void Tap_ShouldNotExecuteAction_WhenResultIsFailure()
    {
        var result = Result<int>.Failure(
            Error.Failure(
                "Test.Error",
                "Something failed."));

        var actionCalled = false;

        var tapped = result.Tap(
            _ => actionCalled = true);

        Assert.False(actionCalled);
        Assert.Same(result, tapped);
    }

    [Fact]
    public void Tap_ShouldThrow_WhenActionIsNull()
    {
        var result = Result<int>.Success(10);

        Action<int> action = null!;

        Assert.Throws<ArgumentNullException>(
            () => result.Tap(action));
    }

    [Fact]
    public async Task TapAsync_ShouldExecuteAction_WhenResultIsSuccess()
    {
        var result = Result<int>.Success(10);
        var capturedValue = 0;

        var tapped = await result.TapAsync(
            value =>
            {
                capturedValue = value;
                return Task.CompletedTask;
            });

        Assert.Equal(10, capturedValue);
        Assert.Same(result, tapped);
    }

    [Fact]
    public async Task TapAsync_ShouldNotExecuteAction_WhenResultIsFailure()
    {
        var result = Result<int>.Failure(
            Error.Failure(
                "Test.Error",
                "Something failed."));

        var actionCalled = false;

        var tapped = await result.TapAsync(
            _ =>
            {
                actionCalled = true;
                return Task.CompletedTask;
            });

        Assert.False(actionCalled);
        Assert.Same(result, tapped);
    }

    [Fact]
    public async Task TapAsync_ShouldThrow_WhenActionIsNull()
    {
        var result = Result<int>.Success(10);

        Func<int, Task> action = null!;

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => result.TapAsync(action));
    }

    [Fact]
    public void Tap_ShouldPropagateException_FromAction()
    {
        var result = Result<int>.Success(10);

        Assert.Throws<InvalidOperationException>(
            () => result.Tap(
                _ => throw new InvalidOperationException(
                    "Tap failed.")));
    }

    [Fact]
    public async Task TapAsync_ShouldPropagateException_FromAction()
    {
        var result = Result<int>.Success(10);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => result.TapAsync(
                _ => Task.FromException(
                    new InvalidOperationException(
                        "Tap failed."))));
    }
}