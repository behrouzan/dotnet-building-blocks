using Behrouzan.BuildingBlocks.AspNetCore.Results;
using Behrouzan.BuildingBlocks.Core.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;


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

    [Fact]
    public void AddBehrouzanResultHttp_ShouldRegisterConfiguredOptions()
    {
        var services = new ServiceCollection();

        services.AddBehrouzanResultHttp(options =>
        {
            options.MapStatusCode(
                ErrorType.NotFound,
                StatusCodes.Status410Gone);
        });

        var provider = services.BuildServiceProvider();

        var options =
            provider
                .GetRequiredService<IOptions<ResultHttpOptions>>()
                .Value;

        Assert.Equal(
            StatusCodes.Status410Gone,
            options.GetStatusCode(ErrorType.NotFound));
    }

    [Fact]
    public void ProblemTypeBase_ShouldHaveDefaultValue()
    {
        var options = new ResultHttpOptions();

        Assert.Equal(
            "urn:behrouzan:problem",
            options.ProblemTypeBase);
    }

    [Fact]
    public void ProblemTypeBase_ShouldBeConfigurable()
    {
        var options = new ResultHttpOptions
        {
            ProblemTypeBase = "https://api.example.com/problems"
        };

        Assert.Equal(
            "https://api.example.com/problems",
            options.ProblemTypeBase);
    }

    [Fact]
    public void ProblemTypeBase_WhenEmpty_ShouldBeInvalid()
    {
        var options = new ResultHttpOptions
        {
            ProblemTypeBase = ""
        };

        Assert.Throws<InvalidOperationException>(() =>
            options.Validate());
    }


    [Fact]
    public void Validate_WhenStatusCodeIsInvalid_ShouldThrow()
    {
        var options = new ResultHttpOptions();

        options.MapStatusCode(
            ErrorType.NotFound,
            999);

        Assert.Throws<InvalidOperationException>(() =>
            options.Validate());
    }

    [Fact]
    public void AddBehrouzanResultHttp_WithInvalidOptions_ShouldFailValidation()
    {
        var services = new ServiceCollection();

        services.AddBehrouzanResultHttp(options =>
        {
            options.ProblemTypeBase = "";
        });

        var provider = services.BuildServiceProvider();

        var options =
            provider.GetRequiredService<IOptions<ResultHttpOptions>>();

        Assert.Throws<OptionsValidationException>(() =>
            _ = options.Value);
    }
}