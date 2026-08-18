using Behrouzan.Results;
using Microsoft.AspNetCore.Mvc;

namespace Behrouzan.Results.AspNetCore;

internal sealed class ResultActionResult<T> : IActionResult
{
    private readonly Result<T> _result;

    public ResultActionResult(
        Result<T> result)
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
                new OkObjectResult(_result.Value);
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