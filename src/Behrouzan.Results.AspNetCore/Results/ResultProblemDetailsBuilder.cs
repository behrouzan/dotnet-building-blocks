using Behrouzan.Results;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Behrouzan.Results.AspNetCore;

/// <summary>
/// Creates Problem Details responses from application errors.
/// </summary>
internal static class ResultProblemDetailsBuilder
{
    /// <summary>
    /// Creates a <see cref="ProblemDetails"/> instance for the specified errors.
    /// </summary>
    /// <param name="errors">
    /// The application errors to include in the problem response.
    /// </param>
    /// <param name="statusCode">
    /// The HTTP status code associated with the errors.
    /// </param>
    /// <param name="options">
    /// The optional HTTP result configuration used when creating the problem details.
    /// When <see langword="null"/>, the default configuration is used.
    /// </param>
    /// <param name="traceId">
    /// The optional request trace identifier to include in the problem details.
    /// </param>
    /// <returns>
    /// A configured <see cref="ProblemDetails"/> instance.
    /// </returns>
    public static ProblemDetails Create(
        IReadOnlyList<Error> errors,
        int statusCode,
        ResultHttpOptions? options = null,
        string? traceId = null)
    {
        ArgumentNullException.ThrowIfNull(errors);

        if (errors.Count == 0)
        {
            throw new ArgumentException(
                "At least one error is required.",
                nameof(errors));
        }

        options ??= new ResultHttpOptions();
        
        var httpErrors = errors
        .Select(error => new HttpError(
            error.Code,
            error.Message,
            error.Type.ToString(),
            error.PropertyPath,
            error.Metadata))
        .ToArray();

        var problemDetails = new ProblemDetails
        {
            Type = ResultHttpMapper.GetProblemType(
                errors,
                options),
            Status = statusCode,
            Title = ResultHttpMapper.GetTitle(errors),
            Detail = errors[0].Message
        };

        problemDetails.Extensions["errors"] = httpErrors;
        
        if (!string.IsNullOrWhiteSpace(traceId))
        {
            problemDetails.Extensions["traceId"] = traceId;
        }

        return problemDetails;
    }
}