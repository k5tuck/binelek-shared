namespace Binah.Infrastructure.MultiTenancy;

public class TenantContext
{
    private static readonly AsyncLocal<Guid?> _tenantId = new();
    
    public static Guid? TenantId
    {
        get => _tenantId.Value;
        set => _tenantId.Value = value;
    }
}
