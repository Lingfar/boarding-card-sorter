using Domain.BoardingCards;
using Domain.BusCards;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Configuration;

namespace Persistence.DbContexts.Configuration;

public class BusCardConfiguration : IEntityTypeConfiguration<BusCard>
{
    public void Configure(EntityTypeBuilder<BusCard> builder)
    {
        // Table
        builder.ToTable(nameof(BoardingCard), Constants.DefaultSchema);

        // Properties
        builder
            .Property(x => x.Seat)
            .HasColumnName(nameof(BusCard.Seat));
    }
}
