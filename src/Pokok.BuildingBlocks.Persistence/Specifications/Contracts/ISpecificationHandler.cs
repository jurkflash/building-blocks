namespace Pokok.BuildingBlocks.Persistence.Specifications.Contracts
{
    public interface ISpecificationHandler<T>
    {
        IQueryable<T> Apply(IQueryable<T> query, ISpecification<T> specification);
    }
}
