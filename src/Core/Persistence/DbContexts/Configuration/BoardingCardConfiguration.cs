using Domain.BoardingCards;
using Domain.BusCards;
using Domain.PlaneCards;
using Domain.TrainCards;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Configuration;

namespace Persistence.DbContexts.Configuration;

public class BoardingCardConfiguration : IEntityTypeConfiguration<BoardingCard>
{
    public void Configure(EntityTypeBuilder<BoardingCard> builder)
    {
        // Table
        builder.ToTable(nameof(BoardingCard), Constants.DefaultSchema);

        // Key
        builder.HasKey(x => x.Id);

        // Discriminator
        builder
            .HasDiscriminator(x => x.Type)
            .HasValue<BusCard>(BoardingCardType.Bus)
            .HasValue<TrainCard>(BoardingCardType.Train)
            .HasValue<PlaneCard>(BoardingCardType.Plane);

        // Properties
        builder
            .Property(x => x.Id)
            .IsRequired()
            .HasColumnName(nameof(BoardingCard.Id))
            .HasDefaultValueSql(Constants.DefaultGuidValueSql);

        builder
            .Property(x => x.Number)
            .IsRequired()
            .HasColumnName(nameof(BoardingCard.Number));

        builder
            .Property(x => x.Departure)
            .IsRequired()
            .HasColumnName(nameof(BoardingCard.Departure));

        builder
            .Property(x => x.Arrival)
            .IsRequired()
            .HasColumnName(nameof(BoardingCard.Arrival));
    }
}
