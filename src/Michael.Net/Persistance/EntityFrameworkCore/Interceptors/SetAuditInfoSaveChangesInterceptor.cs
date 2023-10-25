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
            
            return base.SavingChanges(eventData, result);
        }
    }
}
