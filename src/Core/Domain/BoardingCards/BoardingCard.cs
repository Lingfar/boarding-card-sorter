namespace Domain.BoardingCards;

public abstract class BoardingCard
{
    public Guid Id { get; set; }
    public abstract BoardingCardType Type { get; }
    public string Number { get; set; } = null!;
    public string Departure { get; set; } = null!;
    public string Arrival { get; set; } = null!;

    public override abstract string ToString();
}
