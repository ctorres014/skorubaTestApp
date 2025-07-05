namespace TenantTest.Data.Entities
{
    public class Tenant
    {
        public int TenantId { get; set; }
        public string Name { get; set; }
        public string Identifier { get; set; }

        public Tenant(int tenantId, string identifier)
        {
            TenantId = tenantId;
            Identifier = identifier;
        }
    }
}
