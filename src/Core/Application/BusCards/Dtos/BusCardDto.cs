using Domain.BoardingCards;

namespace Application.BusCards.Dtos;

public sealed record BusCardDto
{
    public Guid Id { get; set; }
    public BoardingCardType Type { get; set; }
    public string Number { get; set; } = null!;
    public string Departure { get; set; } = null!;
    public string Arrival { get; set; } = null!;
    public string? Seat { get; set; }
}
