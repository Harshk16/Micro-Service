using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Service.Infrastructure.Persistence.Configurations;
using Product.Service.Common.Behaviors;

namespace Order.Service.Infrastructure;

public static class DependencyInjection
{
   public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCommonServices();
        services.AddInfrastructure(configuration);

        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Read database connection string and register PostgreSQL provider
        var connectionString = configuration.GetConnectionString("Database");

        services.AddDbContext<OrderDbContext>(options =>
            options.UseNpgsql(connectionString, b => 
                b.MigrationsAssembly(typeof(OrderDbContext).Assembly.FullName)));

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
}
