using Domain.BoardingCards;

namespace Domain.TrainCards;

public sealed class TrainCard : BoardingCard
{
    public required string Seat { get; set; }

    public override string ToString() => $"Take train {Number} from {Departure} to {Arrival}. Sit in seat {Seat}.";
}
