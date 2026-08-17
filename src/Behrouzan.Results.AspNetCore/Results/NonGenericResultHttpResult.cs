using Behrouzan.Results;
using Microsoft.AspNetCore.Http;


namespace Behrouzan.Results.AspNetCore;

/// <summary>
/// Represents an ASP.NET Core HTTP result backed by a non-generic application result.
/// </summary>
internal sealed class NonGenericResultHttpResult : IResult
{
    private readonly Result _result;

    public NonGenericResultHttpResult(
        Result result)
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
                Microsoft.AspNetCore.Http.Results.NoContent();
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