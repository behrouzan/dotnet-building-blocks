using Behrouzan.Results;
using Microsoft.AspNetCore.Http;

namespace Behrouzan.Results.AspNetCore;

internal static class ResultHttpFailureFactory
{
    public static IResult Create(
        IReadOnlyList<Error> errors,
        HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(errors);
        ArgumentNullException.ThrowIfNull(httpContext);

        var problemDetails =
            ResultProblemDetailsFactory.Create(
                errors,
                httpContext);

        return Microsoft.AspNetCore.Http.Results.Problem(
            problemDetails);
    }
}