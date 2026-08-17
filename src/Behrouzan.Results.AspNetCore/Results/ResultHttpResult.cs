using Behrouzan.Results;
using Microsoft.AspNetCore.Http;

namespace Behrouzan.Results.AspNetCore;

/// <summary>
/// Represents an ASP.NET Core HTTP result backed by an application result.
/// </summary>
/// <typeparam name="T">
/// The type of value contained in a successful result.
/// </typeparam>
internal sealed class ResultHttpResult<T> : IResult
{
    private readonly Result<T> _result;

    public ResultHttpResult(Result<T> result)
    {
        ArgumentNullException.ThrowIfNull(result);

        _result = result;
    }

    public async Task ExecuteAsync(
        HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        IResult httpResult;

        if (_result.IsSuccess)
        {
            httpResult =
                Microsoft.AspNetCore.Http.Results.Ok(
                    _result.Value);
        }
        else
        {
            httpResult =
                ResultHttpFailureFactory.Create(
                    _result.Errors,
                    httpContext);
        }

        await httpResult.ExecuteAsync(httpContext);
    }
}