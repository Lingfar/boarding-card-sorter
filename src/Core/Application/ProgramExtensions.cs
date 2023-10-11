using Application.Common.Middlewares;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ProgramExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        var applicationAssembly = typeof(ProgramExtensions).Assembly;
        services
            .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(applicationAssembly))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddValidatorsFromAssembly(applicationAssembly);
        // Disable FluentValidation's language manager to force it to use the default language
        ValidatorOptions.Global.LanguageManager.Enabled = false;
    }
}
