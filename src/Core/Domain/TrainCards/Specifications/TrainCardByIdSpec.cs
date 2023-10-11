using Ardalis.Specification;

namespace Domain.TrainCards.Specifications;

public sealed class TrainCardByIdSpec : Specification<TrainCard>
{
    public TrainCardByIdSpec(Guid id)
    {
        Query.Where(card => card.Id == id);
    }
}
