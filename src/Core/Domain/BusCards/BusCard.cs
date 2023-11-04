using Domain.BoardingCards;

namespace Domain.BusCards;

public sealed class BusCard : BoardingCard
{
    public string? Seat { get; set; }

    public override string ToString() => $"Take bus {Number} from {Departure} to {Arrival}. {SeatAssignment()}";

    private string SeatAssignment() => string.IsNullOrWhiteSpace(Seat) ? "No seat assignment." : $"Sit in seat {Seat}.";
}
