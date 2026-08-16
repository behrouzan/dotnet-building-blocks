using Microsoft.Extensions.DependencyInjection;

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

        services.AddSingleton(new ResultHttpOptions());

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

        var options = new ResultHttpOptions();

        configure(options);

        services.AddSingleton(options);

        return services;
    }
}