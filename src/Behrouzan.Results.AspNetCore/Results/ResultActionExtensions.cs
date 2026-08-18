using Behrouzan.Results;
using Microsoft.AspNetCore.Mvc;

namespace Behrouzan.Results.AspNetCore;

/// <summary>
/// Provides ASP.NET Core controller conversion helpers for application results.
/// </summary>
public static class ResultActionExtensions
{
    /// <summary>
    /// Converts a <see cref="Result{T}"/> into an ASP.NET Core
    /// <see cref="IActionResult"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of value contained in a successful result.
    /// </typeparam>
    /// <param name="result">
    /// The application result to convert.
    /// </param>
    /// <returns>
    /// An action result that returns the successful value with HTTP 200,
    /// or a Problem Details response when the result has failed.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="result"/> is <see langword="null"/>.
    /// </exception>
    public static IActionResult ToActionResult<T>(
        this Result<T> result)
    {
        ArgumentNullException.ThrowIfNull(result);

        return new ResultActionResult<T>(result);
    }

    /// <summary>
    /// Converts a non-generic <see cref="Result"/> into an ASP.NET Core
    /// <see cref="IActionResult"/>.
    /// </summary>
    /// <param name="result">
    /// The application result to convert.
    /// </param>
    /// <returns>
    /// An action result that returns HTTP 204 when successful,
    /// or a Problem Details response when the result has failed.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="result"/> is <see langword="null"/>.
    /// </exception>
    public static IActionResult ToActionResult(
        this Result result)
    {
        ArgumentNullException.ThrowIfNull(result);

        return new NonGenericResultActionResult(result);
    }
}