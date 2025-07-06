using Microsoft.EntityFrameworkCore;
using Pokok.BuildingBlocks.Persistence.Abstractions;

namespace Pokok.BuildingBlocks.Persistence.Base
{
    public class UnitOfWorkBase : IUnitOfWork
    {
        private readonly DbContext _context;

        public UnitOfWorkBase(DbContext context)
        {
            _context = context;
        }

        public Task<int> CompleteAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
