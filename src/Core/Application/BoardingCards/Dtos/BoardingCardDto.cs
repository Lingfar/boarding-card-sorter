using Domain.BoardingCards;

namespace Application.BoardingCards.Dtos;

public sealed record BoardingCardDto
{
    public Guid Id { get; set; }
    public BoardingCardType Type { get; set; }
    public string Number { get; set; } = null!;
    public string Departure { get; set; } = null!;
    public string Arrival { get; set; } = null!;
}
