using Behrouzan.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Behrouzan.Results.AspNetCore;

internal static class ResultProblemDetailsFactory
{
    public static ProblemDetails Create(
        IReadOnlyList<Error> errors,
        HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(errors);
        ArgumentNullException.ThrowIfNull(httpContext);

        var options =
            httpContext.RequestServices
                .GetService<IOptions<ResultHttpOptions>>()
                ?.Value
            ?? new ResultHttpOptions();

        var statusCode =
            ResultHttpMapper.GetStatusCode(
                errors,
                options);

        return ResultProblemDetailsBuilder.Create(
            errors,
            statusCode,
            options,
            httpContext.TraceIdentifier);
    }
}