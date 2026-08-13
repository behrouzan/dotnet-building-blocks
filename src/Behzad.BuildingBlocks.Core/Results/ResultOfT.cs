namespace Behzad.BuildingBlocks.Core.Results;

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
    public Result<TNewValue> Bind<TNewValue>(
        Func<T, Result<TNewValue>> binder)
    {
        ArgumentNullException.ThrowIfNull(binder);

        return IsSuccess
            ? binder(Value)
            : Result<TNewValue>.Failure(Errors);
    }
}