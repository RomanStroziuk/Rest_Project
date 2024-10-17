﻿using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
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


    }

    
    
}