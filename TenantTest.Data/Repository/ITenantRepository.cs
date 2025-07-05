using TenantTest.Data.Entities;

namespace TenantTest.Data.Repository
{
    public interface ITenantRepository<T> where T : Tenant
    {
        Task<T> GetTenantAsync(string identifier);
    }
}
