using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Diagnostics.CodeAnalysis;

namespace Dev.ConsoleApp.Entities
{
    public class AdminDbContext
        : DbContextBase<AdminDbContext>
    {
        public AdminDbContext([NotNull] DbContextOptions<AdminDbContext> options) : base(options)
        {
        }

        protected override void OnSavingChanges(EntityEntry entry)
        {

        }
    }
}
