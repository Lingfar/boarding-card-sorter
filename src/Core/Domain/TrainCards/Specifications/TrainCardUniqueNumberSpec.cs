using Ardalis.Specification;

namespace Domain.TrainCards.Specifications;

public sealed class TrainCardUniqueNumberSpec : Specification<TrainCard>
{
    public TrainCardUniqueNumberSpec(string number)
    {
        Query.Where(card => card.Number == number);
    }

    public TrainCardUniqueNumberSpec(string number, Guid id) : this(number)
    {
        Query.Where(card => card.Id != id);
    }
}
