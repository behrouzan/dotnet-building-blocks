using Behrouzan.Results;

namespace Behrouzan.Results.Tests;

public sealed class ResultTaskExtensionsTests
{
    [Fact]
    public async Task MapAsync_ShouldMapValue_WhenTaskResultIsSuccess()
    {
        Task<Result<int>> resultTask =
            Task.FromResult(
                Result<int>.Success(10));

        var mapped =
            await resultTask.MapAsync(
                value => Task.FromResult(value * 2));

        Assert.True(mapped.IsSuccess);
        Assert.Equal(20, mapped.Value);
    }

    [Fact]
    public async Task MapAsync_ShouldPreserveErrors_WhenTaskResultIsFailure()
    {
        var error =
            Error.Failure(
                "Test.Error",
                "Something failed.");

        Task<Result<int>> resultTask =
            Task.FromResult(
                Result<int>.Failure(error));

        var mapperCalled = false;

        var mapped =
            await resultTask.MapAsync(value =>
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
    public async Task MapAsync_ShouldThrow_WhenResultTaskIsNull()
    {
        Task<Result<int>> resultTask = null!;

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => resultTask.MapAsync(
                value => Task.FromResult(value * 2)));
    }

    [Fact]
    public async Task MapAsync_ShouldThrow_WhenMapperIsNull()
    {
        Task<Result<int>> resultTask =
            Task.FromResult(
                Result<int>.Success(10));

        Func<int, Task<int>> mapper = null!;

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => resultTask.MapAsync(mapper));
    }

    [Fact]
    public async Task BindAsync_ShouldReturnBoundResult_WhenTaskResultIsSuccess()
    {
        Task<Result<int>> resultTask =
            Task.FromResult(
                Result<int>.Success(10));

        var bound =
            await resultTask.BindAsync(
                value => Task.FromResult(
                    Result<string>.Success(
                        value.ToString())));

        Assert.True(bound.IsSuccess);
        Assert.Equal("10", bound.Value);
    }

    [Fact]
    public async Task BindAsync_ShouldPreserveErrors_WhenTaskResultIsFailure()
    {
        var error =
            Error.Failure(
                "Test.Error",
                "Something failed.");

        Task<Result<int>> resultTask =
            Task.FromResult(
                Result<int>.Failure(error));

        var binderCalled = false;

        var bound =
            await resultTask.BindAsync(value =>
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
    public async Task BindAsync_ShouldThrow_WhenResultTaskIsNull()
    {
        Task<Result<int>> resultTask = null!;

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => resultTask.BindAsync(
                value => Task.FromResult(
                    Result<string>.Success(
                        value.ToString()))));
    }

    [Fact]
    public async Task BindAsync_ShouldThrow_WhenBinderIsNull()
    {
        Task<Result<int>> resultTask =
            Task.FromResult(
                Result<int>.Success(10));

        Func<int, Task<Result<string>>> binder = null!;

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => resultTask.BindAsync(binder));
    }

    [Fact]
    public async Task Ensure_ShouldReturnSuccess_WhenPredicateIsTrue()
    {
        Task<Result<int>> resultTask =
            Task.FromResult(
                Result<int>.Success(10));

        var ensured =
            await resultTask.Ensure(
                value => value > 0,
                Error.Validation(
                    "Value.Invalid",
                    "Value must be greater than zero."));

        Assert.True(ensured.IsSuccess);
        Assert.Equal(10, ensured.Value);
    }

    [Fact]
    public async Task Ensure_ShouldReturnFailure_WhenPredicateIsFalse()
    {
        var error =
            Error.Validation(
                "Value.Invalid",
                "Value must be greater than zero.");

        Task<Result<int>> resultTask =
            Task.FromResult(
                Result<int>.Success(0));

        var ensured =
            await resultTask.Ensure(
                value => value > 0,
                error);

        Assert.True(ensured.IsFailure);
        Assert.Single(ensured.Errors);
        Assert.Equal(error, ensured.Errors[0]);
    }

    [Fact]
    public async Task Ensure_ShouldPreserveExistingFailure()
    {
        var originalError =
            Error.NotFound(
                "Product.NotFound",
                "Product was not found.");

        Task<Result<int>> resultTask =
            Task.FromResult(
                Result<int>.Failure(originalError));

        var predicateCalled = false;

        var ensured =
            await resultTask.Ensure(
                value =>
                {
                    predicateCalled = true;
                    return value > 0;
                },
                Error.Validation(
                    "Value.Invalid",
                    "Value is invalid."));

        Assert.True(ensured.IsFailure);
        Assert.False(predicateCalled);
        Assert.Equal(originalError, ensured.Errors[0]);
    }

    [Fact]
    public async Task Ensure_ShouldThrow_WhenResultTaskIsNull()
    {
        Task<Result<int>> resultTask = null!;

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => resultTask.Ensure(
                value => value > 0,
                Error.Validation(
                    "Value.Invalid",
                    "Value is invalid.")));
    }

    [Fact]
    public async Task Ensure_ShouldThrow_WhenPredicateIsNull()
    {
        Task<Result<int>> resultTask =
            Task.FromResult(
                Result<int>.Success(10));

        Func<int, bool> predicate = null!;

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => resultTask.Ensure(
                predicate,
                Error.Validation(
                    "Value.Invalid",
                    "Value is invalid.")));
    }

    [Fact]
    public async Task Ensure_ShouldThrow_WhenErrorIsNull()
    {
        Task<Result<int>> resultTask =
            Task.FromResult(
                Result<int>.Success(10));

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => resultTask.Ensure(
                value => value > 0,
                null!));
    }

    [Fact]
    public async Task Map_ShouldMapValue_WhenTaskResultIsSuccess()
    {
        Task<Result<int>> resultTask =
            Task.FromResult(
                Result<int>.Success(10));

        var mapped =
            await resultTask.Map(
                value => value * 2);

        Assert.True(mapped.IsSuccess);
        Assert.Equal(20, mapped.Value);
    }

    [Fact]
    public async Task Map_ShouldPreserveErrors_WhenTaskResultIsFailure()
    {
        var error =
            Error.Failure(
                "Test.Error",
                "Something failed.");

        Task<Result<int>> resultTask =
            Task.FromResult(
                Result<int>.Failure(error));

        var mapperCalled = false;

        var mapped =
            await resultTask.Map(value =>
            {
                mapperCalled = true;
                return value * 2;
            });

        Assert.True(mapped.IsFailure);
        Assert.False(mapperCalled);
        Assert.Equal(error, mapped.Errors[0]);
    }

    [Fact]
    public async Task Map_ShouldThrow_WhenResultTaskIsNull()
    {
        Task<Result<int>> resultTask = null!;

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => resultTask.Map(
                value => value * 2));
    }

    [Fact]
    public async Task Map_ShouldThrow_WhenMapperIsNull()
    {
        Task<Result<int>> resultTask =
            Task.FromResult(
                Result<int>.Success(10));

        Func<int, int> mapper = null!;

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => resultTask.Map(mapper));
    }

    [Fact]
    public async Task Bind_ShouldReturnBoundResult_WhenTaskResultIsSuccess()
    {
        Task<Result<int>> resultTask =
            Task.FromResult(
                Result<int>.Success(10));

        var bound =
            await resultTask.Bind(
                value =>
                    Result<string>.Success(
                        value.ToString()));

        Assert.True(bound.IsSuccess);
        Assert.Equal("10", bound.Value);
    }

    [Fact]
    public async Task Bind_ShouldPreserveErrors_WhenTaskResultIsFailure()
    {
        var error =
            Error.Failure(
                "Test.Error",
                "Something failed.");

        Task<Result<int>> resultTask =
            Task.FromResult(
                Result<int>.Failure(error));

        var binderCalled = false;

        var bound =
            await resultTask.Bind(value =>
            {
                binderCalled = true;

                return Result<string>.Success(
                    value.ToString());
            });

        Assert.True(bound.IsFailure);
        Assert.False(binderCalled);
        Assert.Equal(error, bound.Errors[0]);
    }

    [Fact]
    public async Task Bind_ShouldThrow_WhenResultTaskIsNull()
    {
        Task<Result<int>> resultTask = null!;

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => resultTask.Bind(
                value =>
                    Result<string>.Success(
                        value.ToString())));
    }

    [Fact]
    public async Task Bind_ShouldThrow_WhenBinderIsNull()
    {
        Task<Result<int>> resultTask =
            Task.FromResult(
                Result<int>.Success(10));

        Func<int, Result<string>> binder = null!;

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => resultTask.Bind(binder));
    }

    [Fact]
    public async Task Tap_ShouldExecuteAction_WhenTaskResultIsSuccess()
    {
        Task<Result<int>> resultTask =
            Task.FromResult(
                Result<int>.Success(10));

        var capturedValue = 0;

        var tapped =
            await resultTask.Tap(
                value => capturedValue = value);

        Assert.Equal(10, capturedValue);
        Assert.True(tapped.IsSuccess);
        Assert.Equal(10, tapped.Value);
    }

    [Fact]
    public async Task Tap_ShouldNotExecuteAction_WhenTaskResultIsFailure()
    {
        var error =
            Error.Failure(
                "Test.Error",
                "Something failed.");

        Task<Result<int>> resultTask =
            Task.FromResult(
                Result<int>.Failure(error));

        var actionCalled = false;

        var tapped =
            await resultTask.Tap(
                _ => actionCalled = true);

        Assert.False(actionCalled);
        Assert.True(tapped.IsFailure);
        Assert.Equal(error, tapped.Errors[0]);
    }

    [Fact]
    public async Task Tap_ShouldThrow_WhenResultTaskIsNull()
    {
        Task<Result<int>> resultTask = null!;

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => resultTask.Tap(
                _ => { }));
    }

    [Fact]
    public async Task TapAsync_ShouldExecuteAction_WhenTaskResultIsSuccess()
    {
        Task<Result<int>> resultTask =
            Task.FromResult(
                Result<int>.Success(10));

        var capturedValue = 0;

        var tapped =
            await resultTask.TapAsync(
                value =>
                {
                    capturedValue = value;

                    return Task.CompletedTask;
                });

        Assert.Equal(10, capturedValue);
        Assert.True(tapped.IsSuccess);
        Assert.Equal(10, tapped.Value);
    }

    [Fact]
    public async Task TapAsync_ShouldNotExecuteAction_WhenTaskResultIsFailure()
    {
        var error =
            Error.Failure(
                "Test.Error",
                "Something failed.");

        Task<Result<int>> resultTask =
            Task.FromResult(
                Result<int>.Failure(error));

        var actionCalled = false;

        var tapped =
            await resultTask.TapAsync(
                _ =>
                {
                    actionCalled = true;

                    return Task.CompletedTask;
                });

        Assert.False(actionCalled);
        Assert.True(tapped.IsFailure);
        Assert.Equal(error, tapped.Errors[0]);
    }

    [Fact]
    public async Task TapAsync_ShouldThrow_WhenResultTaskIsNull()
    {
        Task<Result<int>> resultTask = null!;

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => resultTask.TapAsync(
                _ => Task.CompletedTask));
    }
}