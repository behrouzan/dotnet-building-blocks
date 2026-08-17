using Behrouzan.BuildingBlocks.AspNetCore.Results;
using Behrouzan.BuildingBlocks.Core.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;


namespace Behrouzan.BuildingBlocks.AspNetCore.Tests;

public class ResultHttpOptionsTests
{

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