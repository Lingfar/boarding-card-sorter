using Application.Common.Interfaces;
using FluentValidation;

namespace Application.BoardingCards.Commands;

public static class BoardingCardSync
{
    public abstract record Command<TResponse> : ICommand<TResponse>
    {
        public string Number { get; set; } = null!;
        public string Departure { get; set; } = null!;
        public string Arrival { get; set; } = null!;
    }

    public abstract class Validator<TCommand, TResponse> : AbstractValidator<TCommand>
        where TCommand : Command<TResponse>
    {
        protected Validator()
        {
            RuleFor(x => x.Number)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.Departure)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.Arrival)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .NotEmpty();
        }
    }
}
