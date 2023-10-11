using Microsoft.EntityFrameworkCore;

namespace Persistence.DbContexts;

public sealed class ReadDbContext : WriteDbContext
{
    private const string ReadOnlyContextMessage = "This context is read-only.";

    public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    public override int SaveChanges()
    {
        throw new InvalidOperationException(ReadOnlyContextMessage);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        throw new InvalidOperationException(ReadOnlyContextMessage);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException(ReadOnlyContextMessage);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException(ReadOnlyContextMessage);
    }
}
