using Application;
using Domain;
using HealthChecks.UI.Client;
using Hellang.Middleware.ProblemDetails;
using Host.Helpers;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Persistence;
using Serilog;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("serilog.json", true, true);
builder.Configuration.AddJsonFile($"serilog.{builder.Environment.EnvironmentName}.json", true, true);
builder.Configuration.AddEnvironmentVariables();

builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));

builder.Services.AddDomain();
builder.Services.AddPersistence();
builder.Services.AddApplication();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddVersionedApiExplorer(options => options.GroupNameFormat = "'v'VVV");
builder.Services.AddApiVersioning(opt =>
{
    opt.ReportApiVersions = true;
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.DefaultApiVersion = new ApiVersion(1, 0);
});

builder.Services
    .AddControllers(options => options.SuppressAsyncSuffixInActionNames = false)
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Used for Authentication, propagations in Interceptors, logs
builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        corsPolicyBuilder =>
        {
            corsPolicyBuilder.AllowAnyOrigin();
            corsPolicyBuilder.AllowAnyHeader();
            corsPolicyBuilder.AllowAnyMethod();
        });
});

builder.Services.AddHealthChecks();
builder.Services.AddProblemDetailsServices();

if (!builder.Environment.IsProduction())
{
    builder.Services.AddSwaggerServices();
}

var app = builder.Build();
app.Logger.LogInformation("Starting up service.");

if (!builder.Environment.IsProduction())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUiServices(builder.Services);
}

app.UseProblemDetails();

app.UseRouting();

app.UseHttpsRedirection();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/healthz", new HealthCheckOptions { ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse, AllowCachingResponses = false });

try
{
    app.Run();
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "Service unexpected crashed.");
    throw;
}
