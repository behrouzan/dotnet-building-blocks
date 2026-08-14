using Behzad.BuildingBlocks.Core.Results;
using Microsoft.AspNetCore.Http;

namespace Behzad.BuildingBlocks.AspNetCore.Results;

/// <summary>
/// Provides ASP.NET Core HTTP conversion helpers for application results.
/// </summary>
public static class ResultHttpExtensions
{
    /// <summary>
    /// Converts a <see cref="Result{T}"/> into an ASP.NET Core <see cref="IResult"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of value contained in a successful result.
    /// </typeparam>
    /// <param name="result">
    /// The application result to convert.
    /// </param>
    /// <returns>
    /// An HTTP result representing either the successful value or the failure.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="result"/> is <see langword="null"/>.
    /// </exception>
    public static IResult ToHttpResult<T>(
        this Result<T> result,
        HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (result.IsSuccess)
        {
            //return Results.Ok(result.Value);
            return Microsoft.AspNetCore.Http.Results.Ok(result.Value);
        }

        var statusCode =
            ResultHttpMapper.GetStatusCode(result.Errors);

        var problemDetails =
        ResultProblemDetailsBuilder.Create(
            result.Errors,
            statusCode,
            httpContext.TraceIdentifier);

        return Microsoft.AspNetCore.Http.Results.Problem(
            problemDetails);
    }
}