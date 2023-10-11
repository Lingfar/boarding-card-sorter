using Ardalis.Specification;

namespace Domain.PlaneCards.Specifications;

public sealed class PlaneCardByIdSpec : Specification<PlaneCard>
{
    public PlaneCardByIdSpec(Guid id)
    {
        Query.Where(card => card.Id == id);
    }
}
