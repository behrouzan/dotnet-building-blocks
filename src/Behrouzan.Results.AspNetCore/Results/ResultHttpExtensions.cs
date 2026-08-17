using Behrouzan.Results;
using Microsoft.AspNetCore.Http;

namespace Behrouzan.Results.AspNetCore;

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
    /// A 200 response containing the successful value, or an HTTP problem response when failed.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="result"/> is <see langword="null"/>.
    /// </exception>
    public static IResult ToHttpResult<T>(
        this Result<T> result)
    {
        ArgumentNullException.ThrowIfNull(result);
        return new ResultHttpResult<T>(result);
    }

    /// <summary>
    /// Converts a non-generic <see cref="Result"/> into an ASP.NET Core <see cref="IResult"/>.
    /// </summary>
    /// <param name="result">
    /// The application result to convert.
    /// </param>
    /// <returns>
    /// A 204 response when successful, or an HTTP problem response when failed.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="result"/> is <see langword="null"/>.
    /// </exception>
    public static IResult ToHttpResult(
        this Result result)
    {
        ArgumentNullException.ThrowIfNull(result);

        return new NonGenericResultHttpResult(result);
    }
}