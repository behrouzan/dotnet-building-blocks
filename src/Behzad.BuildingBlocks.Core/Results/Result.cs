namespace Behzad.BuildingBlocks.Core.Results;

/// <summary>
/// Represents the outcome of an operation that does not return a value.
/// </summary>
public class Result
{
    private readonly Error[] _errors;

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    /// <param name="isSuccess">
    /// Indicates whether the operation completed successfully.
    /// </param>
    /// <param name="errors">
    /// The errors produced by the operation.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a successful result contains errors,
    /// or when a failed result contains no errors.
    /// </exception>
    protected Result(
        bool isSuccess,
        IEnumerable<Error>? errors = null)
    {
        _errors = errors?.ToArray() ?? [];

        if (isSuccess && _errors.Length > 0)
        {
            throw new InvalidOperationException(
                "A successful result cannot contain errors.");
        }

        if (!isSuccess && _errors.Length == 0)
        {
            throw new InvalidOperationException(
                "A failed result must contain at least one error.");
        }

        IsSuccess = isSuccess;
    }

    /// <summary>
    /// Gets a value indicating whether the operation completed successfully.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the errors produced by the operation.
    /// </summary>
    /// <remarks>
    /// A successful result contains no errors, while a failed result
    /// contains at least one error.
    /// </remarks>
    public IReadOnlyList<Error> Errors => _errors;

    /// <summary>
    /// Gets the first error, or <see langword="null"/> if the result is successful.
    /// </summary>
    public Error? FirstError =>
        _errors.Length > 0
            ? _errors[0]
            : null;

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <returns>
    /// A successful <see cref="Result"/>.
    /// </returns>
    public static Result Success() =>
        new(true);

    /// <summary>
    /// Creates a failed result containing a single error.
    /// </summary>
    /// <param name="error">
    /// The error that caused the operation to fail.
    /// </param>
    /// <returns>
    /// A failed <see cref="Result"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="error"/> is <see langword="null"/>.
    /// </exception>
    public static Result Failure(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);

        return new Result(
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
    /// A failed <see cref="Result"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="errors"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the error collection is empty.
    /// </exception>
    public static Result Failure(
        IEnumerable<Error> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);

        return new Result(
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
    /// A failed <see cref="Result"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="errors"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the error array is empty.
    /// </exception>
    public static Result Failure(
        params Error[] errors) =>
        Failure((IEnumerable<Error>)errors);
}