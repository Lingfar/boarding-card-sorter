using Domain.BoardingCards;

namespace Domain.PlaneCards;

public sealed class PlaneCard : BoardingCard
{
    public required string Seat { get; set; }
    public required string Gate { get; set; }
    public string? Counter { get; set; }

    public override string ToString() => $"From {Departure}, take flight {Number} to {Arrival}. Gate {Gate}, seat {Seat}. {BaggageAssignment()}";

    private string BaggageAssignment() => !string.IsNullOrWhiteSpace(Counter)
        ? $"Baggage drop at ticket counter {Counter}."
        : "Baggage will be automatically transferred from your last leg.";
}
