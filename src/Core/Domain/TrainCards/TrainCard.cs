using Domain.BoardingCards;

namespace Domain.TrainCards;

public sealed class TrainCard : BoardingCard
{
    public override BoardingCardType Type { get; } = BoardingCardType.Train;
    public string Seat { get; set; } = null!;

    public override string ToString() => $"Take train {Number} from {Departure} to {Arrival}. Sit in seat {Seat}.";
}
