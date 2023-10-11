using Domain.BoardingCards;
using Domain.BusCards;
using Domain.PlaneCards;
using Domain.TrainCards;
using Microsoft.EntityFrameworkCore;

namespace Persistence.DbContexts;

public class WriteDbContext : DbContext
{
    public DbSet<BoardingCard> BoardingCards => Set<BoardingCard>();
    public DbSet<BusCard> BusCards => Set<BusCard>();
    public DbSet<TrainCard> TrainCards => Set<TrainCard>();
    public DbSet<PlaneCard> PlaneCards => Set<PlaneCard>();

    public WriteDbContext(DbContextOptions<WriteDbContext> options) : base(options)
    {
    }

    protected WriteDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(WriteDbContext).Assembly);
}
