using Dumpify;
using Finbuckle.MultiTenant.Abstractions;
using FinbuckleMultiTenant.Models;
using Microsoft.EntityFrameworkCore;
namespace FinbuckleMultiTenant.Data;

public class AppDbContext(
    IMultiTenantContextAccessor<AppTenantInfo> multiTenantContextAccessor, 
    IConfiguration configuration)
    : DbContext
{
    // AppTenantInfo is the app's custom implementation of ITenantInfo which 
    private AppTenantInfo? TenantInfo { get; set; } = multiTenantContextAccessor.MultiTenantContext.TenantInfo!;

    // get the current tenant info at the time of construction

    protected override async void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // use the connection string to connect to the per-tenant database
        TenantInfo.Dump();
        optionsBuilder.UseNpgsql(TenantInfo == null ? configuration.GetConnectionString("DefaultConnection") : TenantInfo.ConnectionString);
        base.OnConfiguring(optionsBuilder);
    }
    
    public DbSet<Blog> Blogs { get; set; }
}