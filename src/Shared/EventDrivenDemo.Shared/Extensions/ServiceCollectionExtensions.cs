using EventDrivenDemo.Shared.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EventDrivenDemo.Shared.Extensions;

/// <summary>
/// Extension methods for service collection to register shared services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds event publishing services to the service collection
    /// </summary>
    public static IServiceCollection AddEventPublishing(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqOptions>(options =>
        {
            var section = configuration.GetSection(RabbitMqOptions.SectionName);
            options.HostName = section.GetValue<string>("HostName") ?? "localhost";
            options.Port = section.GetValue<int>("Port", 5672);
            options.UserName = section.GetValue<string>("UserName") ?? "guest";
            options.Password = section.GetValue<string>("Password") ?? "guest";
            options.VirtualHost = section.GetValue<string>("VirtualHost") ?? "/";
            options.ExchangeName = section.GetValue<string>("ExchangeName") ?? "eventdriven.exchange";
        });
        services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();

        return services;
    }
    
    /// <summary>
    /// Adds event handler to the service collection
    /// </summary>
    public static IServiceCollection AddEventHandler<THandler, TEvent>(this IServiceCollection services)
        where THandler : class, IEventHandler<TEvent>
        where TEvent : class, Events.IEvent
    {
        services.AddScoped<IEventHandler<TEvent>, THandler>();
        return services;
    }
}
