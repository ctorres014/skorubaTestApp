using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TenantTest.Data.Context;
using TenantTest.Data.Entities;

namespace TenantTest.Data.Repository
{
    public class TenantRepository : ITenantRepository<Tenant>
    {
        private readonly TenantAdminDbContext _context;
        private readonly IMemoryCache _cache;

        public TenantRepository(TenantAdminDbContext dbContext, IMemoryCache cache)
        {
            _context = dbContext;
            _cache = cache;
        }

        public async Task<Tenant> GetTenantAsync(string identifier)
        {
            var cacheKey = $"Cache_{identifier}";
            var tenant = _cache.Get<Tenant>(cacheKey);

            if (tenant is null)
            {
                var entity = await _context.Tenants
                    .FirstOrDefaultAsync(q => q.Identifier == identifier)
                        ?? throw new ArgumentException($"identifier no es un tenant válido");

                tenant = new Tenant(entity.TenantId, entity.Identifier);

                tenant.Name = entity.Name;

                _cache.Set(cacheKey, tenant);
            }

            return tenant;
        }
    }
}
