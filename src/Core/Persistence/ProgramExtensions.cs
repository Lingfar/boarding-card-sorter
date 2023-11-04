using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Persistence.Configuration;
using Persistence.DbContexts;
using Persistence.DbContexts.Extensions;

namespace Persistence;

public static class ProgramExtensions
{
    public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var dataSource = GetDataSource(configuration.GetConnectionString(Constants.WriteConnectionString));
        services.AddDbContext<WriteDbContext>(opts => opts.UseNpgsql(dataSource));

        var readDataSource = GetDataSource(configuration.GetConnectionString(Constants.ReadConnectionString));
        services.AddDbContext<ReadDbContext>(opts => opts.UseNpgsql(readDataSource));
    }

    private static NpgsqlDataSource GetDataSource(string? connectionString)
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.RegisterEnums();
        return dataSourceBuilder.Build();
    }
}
