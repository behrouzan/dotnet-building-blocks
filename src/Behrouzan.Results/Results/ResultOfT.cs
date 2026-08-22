namespace Behrouzan.Results;

/// <summary>
/// Represents the outcome of an operation that returns a value when successful.
/// </summary>
/// <typeparam name="T">
/// The type of value returned by a successful operation.
/// </typeparam>
public sealed class Result<T> : Result
{
    private readonly T? _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{T}"/> class.
    /// </summary>
    /// <param name="value">
    /// The value produced by the operation when successful.
    /// </param>
    /// <param name="isSuccess">
    /// Indicates whether the operation completed successfully.
    /// </param>
    /// <param name="errors">
    /// The errors produced by the operation when it failed.
    /// </param>
    private Result(
        T? value,
        bool isSuccess,
        IEnumerable<Error>? errors = null)
        : base(isSuccess, errors)
    {
        _value = value;
    }

    /// <summary>
    /// Gets the value produced by a successful operation.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the value is accessed on a failed result.
    /// </exception>
    public T Value =>
        IsSuccess
            ? _value!
            : throw new InvalidOperationException(
                "The value of a failed result cannot be accessed.");

    /// <summary>
    /// Creates a successful result containing the specified value.
    /// </summary>
    /// <param name="value">
    /// The value produced by the operation.
    /// </param>
    /// <returns>
    /// A successful <see cref="Result{T}"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    public static Result<T> Success(T value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return new Result<T>(
            value,
            true);
    }

    /// <summary>
    /// Creates a failed result containing a single error.
    /// </summary>
    /// <param name="error">
    /// The error that caused the operation to fail.
    /// </param>
    /// <returns>
    /// A failed <see cref="Result{T}"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="error"/> is <see langword="null"/>.
    /// </exception>
    public new static Result<T> Failure(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);

        return new Result<T>(
            default,
            false,
            [error]);
    }

    /// <summary>
    /// Creates a failed result containing multiple errors.
    /// </summary>
    /// <param name="errors">
    /// The errors that caused the operation to fail.
    /// </param>
    /// <returns>
    /// A failed <see cref="Result{T}"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="errors"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the error collection is empty.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the error collection contains a null value.
    /// </exception>
    public new static Result<T> Failure(
        IEnumerable<Error> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);

        return new Result<T>(
            default,
            false,
            errors);
    }

    /// <summary>
    /// Creates a failed result containing one or more errors.
    /// </summary>
    /// <param name="errors">
    /// The errors that caused the operation to fail.
    /// </param>
    /// <returns>
    /// A failed <see cref="Result{T}"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="errors"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the error array is empty.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the error array contains a null value.
    /// </exception>
    public new static Result<T> Failure(
        params Error[] errors)
    {
        return Failure((IEnumerable<Error>)errors);
    }


    /// <summary>
    /// Executes one of the provided functions depending on whether the result
    /// is successful or failed.
    /// </summary>
    /// <typeparam name="TResult">
    /// The type returned by the selected function.
    /// </typeparam>
    /// <param name="onSuccess">
    /// The function executed when the result is successful.
    /// </param>
    /// <param name="onFailure">
    /// The function executed when the result is failed.
    /// </param>
    /// <returns>
    /// The value returned by either <paramref name="onSuccess"/>
    /// or <paramref name="onFailure"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="onSuccess"/> or
    /// <paramref name="onFailure"/> is <see langword="null"/>.
    /// </exception>
    public TResult Match<TResult>(
        Func<T, TResult> onSuccess,
        Func<IReadOnlyList<Error>, TResult> onFailure)
    {
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);

        return IsSuccess
            ? onSuccess(Value)
            : onFailure(Errors);
    }


    /// <summary>
    /// Transforms the value of a successful result into a new value.
    /// </summary>
    /// <typeparam name="TNewValue">
    /// The type of the transformed value.
    /// </typeparam>
    /// <param name="mapper">
    /// The function used to transform the successful value.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TNewValue}"/> containing the transformed value
    /// when successful, or the existing errors when failed.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="mapper"/> is <see langword="null"/>.
    /// </exception>
    public Result<TNewValue> Map<TNewValue>(
        Func<T, TNewValue> mapper)
    {
        ArgumentNullException.ThrowIfNull(mapper);

        return IsSuccess
            ? Result<TNewValue>.Success(mapper(Value))
            : Result<TNewValue>.Failure(Errors);
    }

    /// <summary>
    /// Asynchronously transforms the value of a successful result into a new value.
    /// </summary>
    /// <typeparam name="TNewValue">
    /// The type of the transformed value.
    /// </typeparam>
    /// <param name="mapper">
    /// The asynchronous function used to transform the successful value.
    /// </param>
    /// <returns>
    /// A task containing a <see cref="Result{TNewValue}"/> with the transformed value
    /// when successful, or the existing errors when failed.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="mapper"/> is <see langword="null"/>.
    /// </exception>
    public async Task<Result<TNewValue>> MapAsync<TNewValue>(
        Func<T, Task<TNewValue>> mapper)
    {
        ArgumentNullException.ThrowIfNull(mapper);

        if (IsFailure)
        {
            return Result<TNewValue>.Failure(Errors);
        }

        var value =
            await mapper(Value);

        return Result<TNewValue>.Success(value);
    }

    /// <summary>
    /// Chains the current successful result to another operation that returns a result.
    /// </summary>
    /// <typeparam name="TNewValue">
    /// The type of value returned by the next operation.
    /// </typeparam>
    /// <param name="binder">
    /// The function executed when the current result is successful.
    /// </param>
    /// <returns>
    /// The result returned by <paramref name="binder"/> when the current result is successful;
    /// otherwise, a failed <see cref="Result{TNewValue}"/> containing the existing errors.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="binder"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <paramref name="binder"/> returns <see langword="null"/>.
    /// </exception>
    public Result<TNewValue> Bind<TNewValue>(
        Func<T, Result<TNewValue>> binder)
    {
        ArgumentNullException.ThrowIfNull(binder);

        if (IsFailure)
        {
            return Result<TNewValue>.Failure(Errors);
        }

        var result = binder(Value);

        return result
            ?? throw new InvalidOperationException(
                "The binder function cannot return null.");
    }

    /// <summary>
    /// Asynchronously chains the current successful result to another operation
    /// that returns a result.
    /// </summary>
    /// <typeparam name="TNewValue">
    /// The type of value returned by the next operation.
    /// </typeparam>
    /// <param name="binder">
    /// The asynchronous function executed when the current result is successful.
    /// </param>
    /// <returns>
    /// A task containing the result returned by <paramref name="binder"/>
    /// when successful, or a failed result containing the existing errors.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="binder"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <paramref name="binder"/> returns <see langword="null"/>.
    /// </exception>
    public async Task<Result<TNewValue>> BindAsync<TNewValue>(
        Func<T, Task<Result<TNewValue>>> binder)
    {
        ArgumentNullException.ThrowIfNull(binder);

        if (IsFailure)
        {
            return Result<TNewValue>.Failure(Errors);
        }

        var result =
            await binder(Value);

        return result
            ?? throw new InvalidOperationException(
                "The binder function cannot return null.");
    }

    /// <summary>
    /// Ensures that the value of a successful result satisfies the specified condition.
    /// </summary>
    /// <param name="predicate">
    /// The condition that the successful value must satisfy.
    /// </param>
    /// <param name="error">
    /// The error returned when the condition is not satisfied.
    /// </param>
    /// <returns>
    /// The current successful result when the condition is satisfied,
    /// the specified failure when it is not satisfied,
    /// or the existing failure when the current result has already failed.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="predicate"/> or
    /// <paramref name="error"/> is <see langword="null"/>.
    /// </exception>
    public Result<T> Ensure(
        Func<T, bool> predicate,
        Error error)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(error);

        if (IsFailure)
        {
            return this;
        }

        return predicate(Value)
            ? this
            : Result<T>.Failure(error);
    }

    /// <summary>
    /// Executes an action using the value of a successful result
    /// without changing the result.
    /// </summary>
    /// <param name="action">
    /// The action to execute when the result is successful.
    /// </param>
    /// <returns>
    /// The current result unchanged.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="action"/> is <see langword="null"/>.
    /// </exception>
    public Result<T> Tap(
        Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        if (IsSuccess)
        {
            action(Value);
        }

        return this;
    }

    /// <summary>
    /// Asynchronously executes an action using the value of a successful result
    /// without changing the result.
    /// </summary>
    /// <param name="action">
    /// The asynchronous action to execute when the result is successful.
    /// </param>
    /// <returns>
    /// A task containing the current result unchanged.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="action"/> is <see langword="null"/>.
    /// </exception>
    public async Task<Result<T>> TapAsync(
        Func<T, Task> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        if (IsSuccess)
        {
            await action(Value);
        }

        return this;
    }
}