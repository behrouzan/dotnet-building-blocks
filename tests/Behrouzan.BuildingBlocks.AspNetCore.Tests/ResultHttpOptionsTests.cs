using Behrouzan.BuildingBlocks.AspNetCore.Results;
using Behrouzan.BuildingBlocks.Core.Results;
using Microsoft.AspNetCore.Http;

namespace Behrouzan.BuildingBlocks.AspNetCore.Tests;

public class ResultHttpOptionsTests
{
    [Fact]
    public void GetStatusCode_ShouldReturnDefaultStatusCode()
    {
        var options = new ResultHttpOptions();

        var statusCode =
            options.GetStatusCode(ErrorType.NotFound);

        Assert.Equal(
            StatusCodes.Status404NotFound,
            statusCode);
    }

    [Fact]
    public void MapStatusCode_ShouldOverrideDefaultStatusCode()
    {
        var options = new ResultHttpOptions();

        options.MapStatusCode(
            ErrorType.Failure,
            StatusCodes.Status422UnprocessableEntity);

        var statusCode =
            options.GetStatusCode(ErrorType.Failure);

        Assert.Equal(
            StatusCodes.Status422UnprocessableEntity,
            statusCode);
    }
}