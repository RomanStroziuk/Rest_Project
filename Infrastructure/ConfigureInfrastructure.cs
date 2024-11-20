using Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Authentication;

namespace Infrastructure;

public static class ConfigureInfrastructure
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration);
        
        services.AddJwtAuthentication(configuration);
    }
    
}