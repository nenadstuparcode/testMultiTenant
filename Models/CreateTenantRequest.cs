namespace FinbuckleMultiTenant.Models;

public record CreateTenantRequest
{
    public string Identifier { get; set; }
    public string Name { get; set; }
}