using FluentValidation;
using FluentValidation.AspNetCore;

namespace Api.Modules;

public static class SetubModule
{

    public static void SetupServices(this IServiceCollection services)
    {
        services.AddValidators();
    }

    private static void AddValidators(this IServiceCollection services)
    {
        services
            .AddFluentValidationAutoValidation()
            .AddValidatorsFromAssemblyContaining<Program>();
    }
}