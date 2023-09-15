namespace Dev.API.MultiTenancy
{
    public interface ITenantAccessor<T> where T : Tenant
    {
        T Tenant { get; }
    }
}
