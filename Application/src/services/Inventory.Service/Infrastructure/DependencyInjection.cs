using System.Reflection;
using FluentValidation;
using Inventory.Service.Common.Behaviors;
using Inventory.Service.Infrastructure.Persistence;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Inventory.Service.Infrastructure.Messaging.ExchangeName;

namespace Inventory.Service.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCommonServices();
        services.AddInfrastructure(configuration);
        services.AddMessaging(configuration, Assembly.GetExecutingAssembly());

        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Read database connection string and register PostgreSQL provider
        var connectionString = configuration.GetConnectionString("Database");

        services.AddDbContext<InventoryDbContext>(options =>
            options.UseNpgsql(connectionString, b =>
                b.MigrationsAssembly(typeof(InventoryDbContext).Assembly.FullName)));

        return services;
    }

    public static IServiceCollection AddCommonServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // 1. Register MediatR and scan the assembly for all Handlers
        // services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddMediatR(cfg =>
       {
           cfg.RegisterServicesFromAssembly(assembly);

           // 🟢 FIX: Register the Validation Behavior globally inside MediatR
           cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
       });

        // 2. Register FluentValidation and scan for all Validator classes
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
