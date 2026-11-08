using System.Reflection;
using FluentValidation;
using Payment.Service.Infrastructure.Messaging.ExchangeName;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Payment.Service.Common.Behaviors;
using Payment.Service.Infrastructure.Persistence.Configurations;

namespace Payment.Service.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext(configuration);
        services.AddMediatR();
        services.AddMessaging(configuration, Assembly.GetExecutingAssembly());
        return services;
    }

    public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database"); 
        services.AddDbContext<PaymentDbContext>(options =>
            options.UseNpgsql(connectionString, b =>
                b.MigrationsAssembly(typeof(PaymentDbContext).Assembly.FullName)));

        return services;
    }

    public static IServiceCollection AddMediatR(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(assembly);

        return services;
    }

    // MASS TRANSIT Configuration
    public static IServiceCollection AddMessaging(
       this IServiceCollection services,
       IConfiguration configuration,
       Assembly serviceAssembly)
    {
        services.AddMassTransit(x =>
        {
            // 1. Format queue names cleanly using kebab-case automatically
            x.SetKebabCaseEndpointNameFormatter();

            // 2. Discover and register all Consumers from the passed assembly
            x.AddConsumers(serviceAssembly);

            // 3. Configure the RabbitMQ Transport layer
            x.UsingRabbitMq((context, cfg) =>
            {
                // Read broker parameters directly from your appsettings blocks
                cfg.Host(
                    configuration["RabbitMq:Host"],
                    "/",
                    h =>
                    {
                        h.Username(
                            configuration["RabbitMq:Username"] ?? "guest");

                        h.Password(
                            configuration["RabbitMq:Password"] ?? "guest");
                    });

                cfg.MessageTopology.SetEntityNameFormatter(new KebabCaseExchangeNameFormatter());

                // 4. Automatically create and bind endpoints for discovered consumers
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
