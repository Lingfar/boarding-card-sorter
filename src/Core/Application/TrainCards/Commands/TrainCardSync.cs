using Application.BoardingCards.Commands;
using FluentValidation;

namespace Application.TrainCards.Commands;

public static class TrainCardSync
{
    public abstract record Command<TResponse> : BoardingCardSync.Command<TResponse>
    {
        public string Seat { get; set; } = null!;
    }

    public abstract class Validator<TCommand, TResponse> : BoardingCardSync.Validator<TCommand, TResponse>
        where TCommand : Command<TResponse>
    {
        protected Validator()
        {
            RuleFor(x => x.Seat)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .NotEmpty();
        }
    }
}
