using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.EntityFrameworkCore.Stores.EFCoreStore;
using FinbuckleMultiTenant.Models;
using Microsoft.EntityFrameworkCore;

namespace FinbuckleMultiTenant.Data;

public class TenantDbContext(DbContextOptions<TenantDbContext> options, IConfiguration config) : EFCoreStoreDbContext<AppTenantInfo>(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Use InMemory, but could be MsSql, Sqlite, MySql, etc...
        optionsBuilder.UseNpgsql(config.GetConnectionString("DefaultConnection"));
        base.OnConfiguring(optionsBuilder);
    }
    
    public DbSet<AppTenantInfo> TenantInfo => Set<AppTenantInfo>();
    
    
}