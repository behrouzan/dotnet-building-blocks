using Behzad.BuildingBlocks.Core.Results;

namespace Behzad.BuildingBlocks.AspNetCore.Results;

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
        IReadOnlyList<Error> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);

        if (errors.Count == 0)
        {
            throw new ArgumentException(
                "At least one error is required to determine an HTTP status code.",
                nameof(errors));
        }

        var type = GetEffectiveType(errors);

        return type switch
        {
            ErrorType.Validation => 400,
            ErrorType.Unauthorized => 401,
            ErrorType.Forbidden => 403,
            ErrorType.NotFound => 404,
            ErrorType.Conflict => 409,
            ErrorType.RateLimit => 429,
            ErrorType.Unavailable => 503,
            ErrorType.Timeout => 504,
            _ => 500
        };
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
}