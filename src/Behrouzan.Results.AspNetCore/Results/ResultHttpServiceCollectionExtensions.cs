using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Behrouzan.Results.AspNetCore;

/// <summary>
/// Provides dependency injection registration for result HTTP services.
/// </summary>
public static class ResultHttpServiceCollectionExtensions
{
    /// <summary>
    /// Registers the services required to convert application results
    /// into ASP.NET Core HTTP results using the default configuration.
    /// </summary>
    /// <param name="services">
    /// The service collection to register the result HTTP services with.
    /// </param>
    /// <returns>
    /// The same service collection so that additional registrations can be chained.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="services"/> is <see langword="null"/>.
    /// </exception>
    public static IServiceCollection AddBehrouzanResultHttp(
        this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        AddValidatedOptions(services);

        return services;
    }

    /// <summary>
    /// Registers the services required to convert application results
    /// into ASP.NET Core HTTP results using custom configuration.
    /// </summary>
    /// <param name="services">
    /// The service collection to register the result HTTP services with.
    /// </param>
    /// <param name="configure">
    /// An action used to configure result-to-HTTP behavior.
    /// </param>
    /// <returns>
    /// The same service collection so that additional registrations can be chained.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="services"/> or
    /// <paramref name="configure"/> is <see langword="null"/>.
    /// </exception>
    public static IServiceCollection AddBehrouzanResultHttp(
        this IServiceCollection services,
        Action<ResultHttpOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        AddValidatedOptions(services)
            .Configure(configure);

        return services;
    }

    private static OptionsBuilder<ResultHttpOptions> AddValidatedOptions(
        IServiceCollection services)
    {
        return services
            .AddOptions<ResultHttpOptions>()
            .Validate(
                options =>
                {
                    try
                    {
                        options.Validate();
                        return true;
                    }
                    catch (InvalidOperationException)
                    {
                        return false;
                    }
                },
                "Result HTTP options are invalid.")
            .ValidateOnStart();
    }
}