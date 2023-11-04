using Domain.BoardingCards;
using Domain.PlaneCards;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Configuration;

namespace Persistence.DbContexts.Configuration;

public class PlaneCardConfiguration : IEntityTypeConfiguration<PlaneCard>
{
    public void Configure(EntityTypeBuilder<PlaneCard> builder)
    {
        // Table
        builder.ToTable(nameof(BoardingCard), Constants.DefaultSchema);

        // Properties
        builder
            .Property(x => x.Seat)
            .IsRequired()
            .HasColumnName(nameof(PlaneCard.Seat));

        builder
            .Property(x => x.Gate)
            .IsRequired()
            .HasColumnName(nameof(PlaneCard.Gate));

        builder
            .Property(x => x.Counter)
            .HasColumnName(nameof(PlaneCard.Counter));
    }
}
