namespace Pokok.BuildingBlocks.Persistence.Abstractions
{
    public interface IUnitOfWork
    {
        Task<int> CompleteAsync(CancellationToken cancellationToken = default);
    }
}
