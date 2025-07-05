using Microsoft.EntityFrameworkCore;
using TenantTest.Data.Entities;

namespace TenantTest.Data.Context
{
    public class TenantAdminDbContext : DbContext
    {
        public TenantAdminDbContext(DbContextOptions<TenantAdminDbContext> options)
        : base(options) { }

        public DbSet<Tenant> Tenants { get; set; }
    }
}
