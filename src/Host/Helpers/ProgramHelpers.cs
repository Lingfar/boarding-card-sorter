using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;

namespace Host.Helpers;

public static class ProgramHelpers
{
    public static void AddProblemDetailsServices(this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
        {
            options.MapToStatusCode<KeyNotFoundException>(StatusCodes.Status404NotFound);
            options.MapToStatusCode<ValidationException>(StatusCodes.Status422UnprocessableEntity);
            options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
        });
    }

    public static void AddSwaggerServices(this IServiceCollection services)
    {
        var versionProvider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

        services.AddSwaggerGen(options =>
        {
            foreach (var apiVersion in versionProvider.ApiVersionDescriptions.Select(x => x.ApiVersion.ToString()))
            {
                options.SwaggerDoc(apiVersion,
                    new OpenApiInfo
                    {
                        Title = $"API {apiVersion}",
                        Version = apiVersion
                    });
            }

            // Add Swagger documentation for each methods that has an ApiVersion attribute
            options.DocInclusionPredicate((version, desc) =>
            {
                if (!desc.TryGetMethodInfo(out var methodInfo))
                {
                    return false;
                }

                // Use method version if present, otherwise use controller version
                var methodVersions = methodInfo.GetApiVersions();
                return methodVersions.Count > 0
                    ? methodVersions.Exists(v => v.ToString() == version)
                    : methodInfo.DeclaringType!.GetApiVersions().Exists(v => v.ToString() == version);
            });

            options.DescribeAllParametersInCamelCase();
            options.UseInlineDefinitionsForEnums();
            options.SupportNonNullableReferenceTypes();

            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(Program).Assembly.GetName().Name}.xml"), true);

            options.CustomSchemaIds(type => type.ToString().Replace("+", "."));
        });
    }

    public static void UseSwaggerUiServices(this WebApplication app, IServiceCollection services)
    {
        var versionProvider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

        app.UseSwaggerUI(options =>
        {
            foreach (var apiVersion in versionProvider.ApiVersionDescriptions.Select(x => x.ApiVersion.ToString()))
            {
                options.SwaggerEndpoint($"/swagger/{apiVersion}/swagger.json", apiVersion);
            }

            options.DocExpansion(DocExpansion.None);
        });
    }

    private static List<ApiVersion> GetApiVersions(this ICustomAttributeProvider customAttributeProvider)
        => customAttributeProvider.GetCustomAttributes(true).OfType<ApiVersionAttribute>().SelectMany(attr => attr.Versions).ToList();
}
