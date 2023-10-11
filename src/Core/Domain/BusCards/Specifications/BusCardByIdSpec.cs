using Ardalis.Specification;

namespace Domain.BusCards.Specifications;

public sealed class BusCardByIdSpec : Specification<BusCard>
{
    public BusCardByIdSpec(Guid id)
    {
        Query.Where(card => card.Id == id);
    }
}
