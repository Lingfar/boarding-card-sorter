using Domain.BoardingCards;
using Domain.TrainCards;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Configuration;

namespace Persistence.DbContexts.Configuration;

public class TrainCardConfiguration : IEntityTypeConfiguration<TrainCard>
{
    public void Configure(EntityTypeBuilder<TrainCard> builder)
    {
        // Table
        builder.ToTable(nameof(BoardingCard), Constants.DefaultSchema);

        // Properties
        builder
            .Property(x => x.Seat)
            .IsRequired()
            .HasColumnName(nameof(TrainCard.Seat));
    }
}
