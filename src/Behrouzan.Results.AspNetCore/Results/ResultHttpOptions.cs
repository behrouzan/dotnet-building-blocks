using Behrouzan.Results;
using Microsoft.AspNetCore.Http;

namespace Behrouzan.Results.AspNetCore;

/// <summary>
/// Configures HTTP behavior for application results.
/// </summary>
public sealed class ResultHttpOptions
{
    private readonly Dictionary<ErrorType, int> _statusCodes = new()
    {
        [ErrorType.Failure] =
            StatusCodes.Status500InternalServerError,

        [ErrorType.Validation] =
            StatusCodes.Status400BadRequest,

        [ErrorType.Unauthorized] =
            StatusCodes.Status401Unauthorized,

        [ErrorType.Forbidden] =
            StatusCodes.Status403Forbidden,

        [ErrorType.NotFound] =
            StatusCodes.Status404NotFound,

        [ErrorType.Conflict] =
            StatusCodes.Status409Conflict,

        [ErrorType.RateLimit] =
            StatusCodes.Status429TooManyRequests,

        [ErrorType.Unavailable] =
            StatusCodes.Status503ServiceUnavailable,

        [ErrorType.Timeout] =
            StatusCodes.Status504GatewayTimeout
    };

    /// <summary>
    /// Sets the HTTP status code associated with the specified error type.
    /// </summary>
    /// <param name="errorType">
    /// The application error type to configure.
    /// </param>
    /// <param name="statusCode">
    /// The HTTP status code to use for the error type.
    /// </param>
    /// /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="statusCode"/> is outside the valid HTTP status code range.
    /// </exception>
    public void MapStatusCode(
        ErrorType errorType,
        int statusCode)
    {
        if (statusCode < 100 || statusCode > 599)
        {
            throw new ArgumentOutOfRangeException(
                nameof(statusCode),
                statusCode,
                "HTTP status code must be between 100 and 599.");
        }

        _statusCodes[errorType] = statusCode;
    }

    /// <summary>
    /// Gets the configured HTTP status code for an error type.
    /// </summary>
    internal int GetStatusCode(
        ErrorType errorType)
    {
        return _statusCodes[errorType];
    }

    /// <summary>
    /// Gets or sets the base URI or URN used to generate Problem Details type identifiers.
    /// </summary>
    /// <remarks>
    /// The default value is <c>urn:behrouzan:problem</c>.
    /// </remarks>
    public string ProblemTypeBase { get; set; }
        = "urn:behrouzan:problem";


    /// <summary>
    /// Validates the current HTTP result configuration.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the configuration contains invalid values.
    /// </exception>
    internal void Validate()
    {
        if (string.IsNullOrWhiteSpace(ProblemTypeBase))
        {
            throw new InvalidOperationException(
                "ProblemTypeBase cannot be null, empty, or whitespace.");
        }

        foreach (var statusCode in _statusCodes.Values)
        {
            if (statusCode < 100 || statusCode > 599)
            {
                throw new InvalidOperationException(
                    $"HTTP status code '{statusCode}' is invalid.");
            }
        }
    }


}