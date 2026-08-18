using Behrouzan.Results;
using Microsoft.AspNetCore.Mvc;

namespace Behrouzan.Results.AspNetCore;

internal sealed class NonGenericResultActionResult : IActionResult
{
    private readonly Result _result;

    public NonGenericResultActionResult(
        Result result)
    {
        ArgumentNullException.ThrowIfNull(result);

        _result = result;
    }

    public async Task ExecuteResultAsync(
        ActionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        IActionResult actionResult;

        if (_result.IsSuccess)
        {
            actionResult =
                new NoContentResult();
        }
        else
        {
            var problemDetails =
                ResultProblemDetailsFactory.Create(
                    _result.Errors,
                    context.HttpContext);

            actionResult =
                new ObjectResult(problemDetails)
                {
                    StatusCode =
                        problemDetails.Status
                };
        }

        await actionResult.ExecuteResultAsync(context);
    }
}