namespace Domain.BoardingCards;

public abstract class BoardingCard
{
    public Guid Id { get; set; }
    public BoardingCardType Type { get; set; }
    public required string Number { get; set; }
    public required string Departure { get; set; }
    public required string Arrival { get; set; }

    public override abstract string ToString();
}
