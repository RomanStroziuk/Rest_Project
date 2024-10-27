using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Orders;
using Domain.SneakerWarehouses;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Infrastructure.Persistence;

public static class ConfigurePersistence
{
    public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var dataSourceBuild = new NpgsqlDataSourceBuilder(configuration.GetConnectionString("Default"));
        dataSourceBuild.EnableDynamicJson();
        var dataSource = dataSourceBuild.Build();

        services.AddDbContext<ApplicationDbContext>(
            options => options
                .UseNpgsql(
                    dataSource,
                    builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))
                .UseSnakeCaseNamingConvention()
                .ConfigureWarnings(w => w.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning)));

        services.AddScoped<ApplicationDbContextInitialiser>();
        services.AddRepositories();
    }
    private static void AddRepositories(this IServiceCollection services)
    {

        services.AddScoped<SneakerRepository>();
        services.AddScoped<ISneakerRepository>(provider => provider.GetRequiredService<SneakerRepository>());
        services.AddScoped<ISneakerQueries>(provider => provider.GetRequiredService<SneakerRepository>());
    
        services.AddScoped<BrandRepository>();
        services.AddScoped<IBrandRepository>(provider => provider.GetRequiredService<BrandRepository>());
        services.AddScoped<IBrandQueries>(provider => provider.GetRequiredService<BrandRepository>());

        services.AddScoped<CategoryRepository>();
        services.AddScoped<ICategoryRepository>(provider => provider.GetRequiredService<CategoryRepository>());
        services.AddScoped<ICategoryQueries>(provider => provider.GetRequiredService<CategoryRepository>());

        services.AddScoped<OrderRepository>();
        services.AddScoped<IOrderRepository>(provider => provider.GetRequiredService<OrderRepository>());
        services.AddScoped<IOrderQueries>(provider => provider.GetRequiredService<OrderRepository>());

        services.AddScoped<OrderItemRepository>();
        services.AddScoped<IOrderItemRepository>(provider => provider.GetRequiredService<OrderItemRepository>());
        services.AddScoped<IOrderItemQueries>(provider => provider.GetRequiredService<OrderItemRepository>());

        services.AddScoped<WarehouseRepository>();
        services.AddScoped<IWarehouseRepository>(provider => provider.GetRequiredService<WarehouseRepository>());
        services.AddScoped<IWarehouseQueries>(provider => provider.GetRequiredService<WarehouseRepository>());

        services.AddScoped<SneakerWarehouseRepository>();
        services.AddScoped<ISneakerWarehouseRepository>(provider => provider.GetRequiredService<SneakerWarehouseRepository>());
        services.AddScoped<ISneakerWarehouseQueries>(provider => provider.GetRequiredService<SneakerWarehouseRepository>());

        services.AddScoped<StatusRepository>();
        services.AddScoped<IStatusRepository>(provider => provider.GetRequiredService<StatusRepository>());
        services.AddScoped<IStatusQueries>(provider => provider.GetRequiredService<StatusRepository>());

        services.AddScoped<RolesRepository>();
        services.AddScoped<IRoleRepository>(provider => provider.GetRequiredService<RolesRepository>());
        services.AddScoped<IRoleQueries>(provider => provider.GetRequiredService<RolesRepository>());

        services.AddScoped<UserRepository>();
        services.AddScoped<IUserRepository>(provider => provider.GetRequiredService<UserRepository>());
        services.AddScoped<IUserQueries>(provider => provider.GetRequiredService<UserRepository>());
    }

    
    
}