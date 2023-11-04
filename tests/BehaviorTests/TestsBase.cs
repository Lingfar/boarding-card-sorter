using Application;
using AutoBogus;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Persistence.DbContexts;

namespace BehaviorTests;

public abstract class TestsBase : IDisposable
{
    private static readonly InMemoryDatabaseRoot _inMemoryDatabaseRoot = new();

    private readonly ReadDbContext _readContext;

    protected readonly IMediator Mediator;
    protected readonly WriteDbContext DbContext;

    static TestsBase()
    {
        ValidatorOptions.Global.LanguageManager.Enabled = false;

        AutoFaker.Configure(builder =>
        {
            builder
                .WithRecursiveDepth(1)
                .WithTreeDepth(1);
        });
    }

    protected TestsBase()
    {
        var databaseName = Guid.NewGuid().ToString();
        var serviceCollection = new ServiceCollection()
            .AddLogging()
            .AddDbContext<WriteDbContext>(options => options.UseInMemoryDatabase(databaseName, _inMemoryDatabaseRoot))
            .AddDbContext<ReadDbContext>(options => options.UseInMemoryDatabase(databaseName, _inMemoryDatabaseRoot));

        serviceCollection.AddDomain();
        serviceCollection.AddApplication();

        var serviceProvider = serviceCollection.BuildServiceProvider();
        Mediator = serviceProvider.GetRequiredService<IMediator>();
        DbContext = serviceProvider.GetRequiredService<WriteDbContext>();
        _readContext = serviceProvider.GetRequiredService<ReadDbContext>();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        DbContext.Database.EnsureDeleted();
        _readContext.Database.EnsureDeleted();
    }
}
