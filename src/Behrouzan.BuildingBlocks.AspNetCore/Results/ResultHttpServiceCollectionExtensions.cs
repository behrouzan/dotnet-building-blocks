using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Behrouzan.BuildingBlocks.AspNetCore.Results;

/// <summary>
/// Provides dependency injection registration for result HTTP services.
/// </summary>
public static class ResultHttpServiceCollectionExtensions
{
    /// <summary>
    /// Registers result HTTP services using the default configuration.
    /// </summary>
    public static IServiceCollection AddBehrouzanResultHttp(
        this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddOptions<ResultHttpOptions>()
            .Validate(
                options =>
                {
                    try
                    {
                        options.Validate();
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
                "Result HTTP options are invalid.")
            .ValidateOnStart();

        return services;
    }

    /// <summary>
    /// Registers result HTTP services using custom configuration.
    /// </summary>
    public static IServiceCollection AddBehrouzanResultHttp(
        this IServiceCollection services,
        Action<ResultHttpOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        services
            .AddOptions<ResultHttpOptions>()
            .Configure(configure)
            .Validate(
                options =>
                {
                    try
                    {
                        options.Validate();
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
                "Result HTTP options are invalid.")
            .ValidateOnStart();

        return services;
    }
}