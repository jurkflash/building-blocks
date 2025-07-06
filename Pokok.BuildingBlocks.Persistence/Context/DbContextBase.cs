using Microsoft.EntityFrameworkCore;
using Pokok.BuildingBlocks.Common;
using Pokok.BuildingBlocks.Persistence.Abstractions;
using Pokok.BuildingBlocks.Persistence.Entities;

namespace Pokok.BuildingBlocks.Persistence.Context
{
    public abstract class DbContextBase : DbContext
    {
        private readonly ICurrentUserService _currentUser;

        protected DbContextBase(DbContextOptions options, ICurrentUserService currentUser) : base(options) 
        {
            _currentUser = currentUser;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<IAuditable>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAtUtc = DateTime.UtcNow;
                    entry.Entity.CreatedBy = _currentUser.UserId;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.ModifiedAtUtc = DateTime.UtcNow;
                    entry.Entity.ModifiedBy = _currentUser.UserId;
                }
            }

            foreach (var entry in ChangeTracker.Entries<ISoftDeletable>())
            {
                if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAtUtc = DateTime.UtcNow;

                    if (entry.Entity is EntityBase entityBase)
                        entityBase.DeletedBy = _currentUser.UserId;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
