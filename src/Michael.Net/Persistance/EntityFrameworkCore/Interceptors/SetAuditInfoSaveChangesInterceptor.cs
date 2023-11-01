using Michael.Net.Domain;
using Michael.Net.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Michael.Net.Persistance.EntityFrameworkCore.Interceptors
{
    public class SetAuditInfoSaveChangesInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            SetAuditedInfo(eventData);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            SetAuditedInfo(eventData);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        void SetAuditedInfo(DbContextEventData eventData)
        {
            var entries = eventData.Context!.ChangeTracker.Entries();

            entries.ForEach(e =>
            {
                var entity = (IAudited)e.Entity;
                if (e.State == EntityState.Added)
                {
                    entity.CreatedBy = 1;
                    entity.CreatedOn = DateTime.UtcNow;
                }
                else if (e.State == EntityState.Modified)
                {
                    entity.ModifiedBy = 1;
                    entity.ModifiedOn = DateTime.UtcNow;
                }
            });
        }
    }
}
