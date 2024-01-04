using Domain.BoardingCards;

namespace Application.PlaneCards.Dtos;

public sealed record PlaneCardDto
{
    public Guid Id { get; set; }
    public BoardingCardType Type { get; set; }
    public string Number { get; set; } = null!;
    public string Departure { get; set; } = null!;
    public string Arrival { get; set; } = null!;
    public string Seat { get; set; } = null!;
    public string Gate { get; set; } = null!;
    public string? Counter { get; set; }
}
