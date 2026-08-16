using Behrouzan.BuildingBlocks.Core.Results;
using Microsoft.AspNetCore.Http;

namespace Behrouzan.BuildingBlocks.AspNetCore.Results;

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
    /// Sets the HTTP status code associated with an error type.
    /// </summary>
    public void MapStatusCode(
        ErrorType errorType,
        int statusCode)
    {
        _statusCodes[errorType] = statusCode;
    }

    /// <summary>
    /// Gets the configured HTTP status code for an error type.
    /// </summary>
    public int GetStatusCode(
        ErrorType errorType)
    {
        return _statusCodes[errorType];
    }
}