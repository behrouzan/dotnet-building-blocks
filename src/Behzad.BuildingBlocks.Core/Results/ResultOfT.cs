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
    public static Result<T> Success(T value)
    {
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
}