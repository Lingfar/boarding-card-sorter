using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Persistence.DbContexts;

namespace Persistence;

public static class ProgramExtensions
{
    private const string DatabaseName = "InMemoryDatabase";
    private static readonly InMemoryDatabaseRoot _inMemoryDatabaseRoot = new();

    public static void AddPersistence(this IServiceCollection services)
    {
        services.AddDbContext<WriteDbContext>(options => options.UseInMemoryDatabase(DatabaseName, _inMemoryDatabaseRoot));
        services.AddDbContext<ReadDbContext>(options => options.UseInMemoryDatabase(DatabaseName, _inMemoryDatabaseRoot));
    }
}
