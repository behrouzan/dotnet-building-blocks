using Behrouzan.BuildingBlocks.Core.Results;

namespace Behrouzan.BuildingBlocks.AspNetCore.Results;

/// <summary>
/// Maps application result errors to HTTP-specific values.
/// </summary>
public static class ResultHttpMapper
{
    /// <summary>
    /// Gets the HTTP status code for the specified errors.
    /// </summary>
    /// <param name="errors">
    /// The errors produced by the application operation.
    /// </param>
    /// <returns>
    /// The HTTP status code associated with the error set.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="errors"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="errors"/> is empty.
    /// </exception>
    public static int GetStatusCode(
        IReadOnlyList<Error> errors,
        ResultHttpOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(errors);

        if (errors.Count == 0)
        {
            throw new ArgumentException(
                "At least one error is required to determine an HTTP status code.",
                nameof(errors));
        }

        options ??= new ResultHttpOptions();
        var type = GetEffectiveType(errors);

        return options.GetStatusCode(type);
    }


    /// <summary>
    /// Gets a human-readable title for the specified errors.
    /// </summary>
    /// <param name="errors">
    /// The errors produced by the application operation.
    /// </param>
    /// <returns>
    /// A short HTTP problem title describing the error set.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="errors"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="errors"/> is empty.
    /// </exception>
    public static string GetTitle(
        IReadOnlyList<Error> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);

        if (errors.Count == 0)
        {
            throw new ArgumentException(
                "At least one error is required to determine a problem title.",
                nameof(errors));
        }

        var type = GetEffectiveType(errors);

        return type switch
        {
            ErrorType.Unauthorized => "Unauthorized",
            ErrorType.Forbidden => "Forbidden",
            ErrorType.Conflict => "Conflict",
            ErrorType.Validation => "Validation failed",
            ErrorType.NotFound => "Resource not found",
            ErrorType.RateLimit => "Too many requests",
            ErrorType.Unavailable => "Service unavailable",
            ErrorType.Timeout => "Request timed out",
            _ => "Request failed"
        };
    }


    private static ErrorType GetEffectiveType(
        IReadOnlyList<Error> errors)
    {
        var types = errors
            .Select(error => error.Type)
            .ToHashSet();

        if (types.Contains(ErrorType.Unauthorized))
            return ErrorType.Unauthorized;

        if (types.Contains(ErrorType.Forbidden))
            return ErrorType.Forbidden;

        if (types.Contains(ErrorType.Conflict))
            return ErrorType.Conflict;

        if (types.Contains(ErrorType.Validation))
            return ErrorType.Validation;

        if (types.Contains(ErrorType.NotFound))
            return ErrorType.NotFound;

        if (types.Contains(ErrorType.RateLimit))
            return ErrorType.RateLimit;

        if (types.Contains(ErrorType.Unavailable))
            return ErrorType.Unavailable;

        if (types.Contains(ErrorType.Timeout))
            return ErrorType.Timeout;

        return ErrorType.Failure;
    }

    /// <summary>
    /// Gets a machine-readable problem type identifier for the specified errors.
    /// </summary>
    /// <param name="errors">
    /// The errors produced by the application operation.
    /// </param>
    /// <returns>
    /// A stable problem type identifier.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="errors"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="errors"/> is empty.
    /// </exception>
    public static string GetProblemType(
        IReadOnlyList<Error> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);

        if (errors.Count == 0)
        {
            throw new ArgumentException(
                "At least one error is required to determine a problem type.",
                nameof(errors));
        }

        var type = GetEffectiveType(errors);

        return type switch
        {
            ErrorType.Validation =>
                "urn:behrouzan:problem:validation",

            ErrorType.Unauthorized =>
                "urn:behrouzan:problem:unauthorized",

            ErrorType.Forbidden =>
                "urn:behrouzan:problem:forbidden",

            ErrorType.NotFound =>
                "urn:behrouzan:problem:not-found",

            ErrorType.Conflict =>
                "urn:behrouzan:problem:conflict",

            ErrorType.RateLimit =>
                "urn:behrouzan:problem:rate-limit",

            ErrorType.Unavailable =>
                "urn:behrouzan:problem:unavailable",

            ErrorType.Timeout =>
                "urn:behrouzan:problem:timeout",

            _ =>
                "urn:behrouzan:problem:failure"
        };
    }


}