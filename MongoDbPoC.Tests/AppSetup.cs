using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDbPoC.Data;
using MongoDbPoC.Data.Entities;
using MongoDbPoC.Data.Repository;

namespace MongoDbPoC.Tests;

public static class AppSetup
{
    public static IServiceCollection RegisterAppConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoOptions>(options =>
        {
            // Set default settings    
            configuration.GetSection("MongoSettings").Bind(options);

            // Check for environment variable and replace in case is informed
            var connectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING");
            if (!string.IsNullOrEmpty(connectionString))
            {
                options.Host = connectionString;
            }
        });

        return services;
    }

    public static IServiceCollection RegisterRepository(this IServiceCollection services)
    {
        var mongoDbOptions = services.BuildServiceProvider().GetRequiredService<IOptions<MongoOptions>>();


        var assembly = typeof(BaseEntity).Assembly;
        var entityTypes = assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(BaseEntity)));

        foreach (var entityType in entityTypes)
        {
            var repositoryType = typeof(IRepository<>).MakeGenericType(entityType);
            var implementationType = typeof(MongoRepository2<>).MakeGenericType(entityType);
            services.AddScoped(repositoryType, implementationType);
        }

        //services.AddScoped<IRepository<UserEntity>>(provider =>
        //{
        //    var userEntityOptions = provider.GetRequiredService<IOptions<UserEntityOptions>>();
        //    return new MongoRepository<UserEntity>(mongoDbOptions, userEntityOptions);
        //});

        //services.AddScoped<IRepository<UserEntity>, MongoRepository2<UserEntity>>();

        return services;
    }
}
