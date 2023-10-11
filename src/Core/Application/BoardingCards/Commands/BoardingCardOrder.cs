using Application.Common.Interfaces;
using Domain.BoardingCards;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.DbContexts;
using System.Text;

namespace Application.BoardingCards.Commands;

public static class BoardingCardOrder
{
    public sealed record Command : ICommand<string>;

    public sealed class Handler(ReadDbContext readDbContext) : IRequestHandler<Command, string>
    {
        public async Task<string> Handle(Command request, CancellationToken cancellationToken)
        {
            var unorderedCards = await readDbContext.BoardingCards.ToListAsync(cancellationToken);

            var cardByDeparture = new Dictionary<string, BoardingCard>();
            var arrivals = new HashSet<string>();

            foreach (var card in unorderedCards)
            {
                if (cardByDeparture.ContainsKey(card.Departure))
                {
                    throw new ValidationException($"Duplicate '{nameof(card.Departure)}' location found: {card.Departure}.");
                }

                cardByDeparture[card.Departure] = card;

                if (!arrivals.Add(card.Arrival))
                {
                    throw new ValidationException($"Duplicate '{nameof(card.Arrival)}' location found: {card.Arrival}.");
                }
            }

            var startLocation = FindStartLocation(cardByDeparture.Keys, arrivals);
            return GetJourney(startLocation, cardByDeparture);
        }

        private static string GetJourney(string startLocation, Dictionary<string, BoardingCard> cardByDeparture)
        {
            var journey = new StringBuilder();

            var currentLocation = startLocation;
            while (cardByDeparture.TryGetValue(currentLocation, out var currentCard))
            {
                journey.AppendLine(currentCard.ToString());
                currentLocation = currentCard.Arrival;
            }

            journey.Append("You have arrived at your final destination.");
            return journey.ToString();
        }

        private static string FindStartLocation(Dictionary<string, BoardingCard>.KeyCollection departures, HashSet<string> arrivals)
        {
            var startLocations = departures.Where(departure => !arrivals.Contains(departure)).ToList();
            return startLocations.Count switch
            {
                0 => throw new ValidationException("Cannot find the start of the journey."),
                > 1 => throw new ValidationException("There are gaps in the journey."),
                _ => startLocations[0]
            };
        }
    }
}
