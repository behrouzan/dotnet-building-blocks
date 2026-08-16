using Behrouzan.BuildingBlocks.Core.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Behrouzan.BuildingBlocks.AspNetCore.Results;

/// <summary>
/// Creates HTTP problem results for failed application results.
/// </summary>
internal static class ResultHttpFailureFactory
{
    public static IResult Create(
        IReadOnlyList<Error> errors,
        HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(errors);
        ArgumentNullException.ThrowIfNull(httpContext);

        var options =
            ServiceProviderServiceExtensions
                .GetService<IOptions<ResultHttpOptions>>(
                    httpContext.RequestServices)
                ?.Value
            ?? new ResultHttpOptions();

        var statusCode =
            ResultHttpMapper.GetStatusCode(
                errors,
                options);

        var problemDetails =
            ResultProblemDetailsBuilder.Create(
                errors,
                statusCode,
                options,
                httpContext.TraceIdentifier);

        return Microsoft.AspNetCore.Http.Results.Problem(
            problemDetails);
    }
}