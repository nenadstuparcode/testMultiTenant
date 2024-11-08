using Finbuckle.MultiTenant.Abstractions;
using FinbuckleMultiTenant.Models;
using Microsoft.EntityFrameworkCore;

namespace FinbuckleMultiTenant.Data.Store;

public class EFCoreStore(TenantDbContext dbContext) : IMultiTenantStore<AppTenantInfo>
{
    
    // ADD
    public async Task<bool> TryAddAsync(AppTenantInfo tenantInfo)
    {
        await dbContext.TenantInfo.AddAsync(tenantInfo);
        var result = await dbContext.SaveChangesAsync() > 0;
        dbContext.Entry(tenantInfo).State = EntityState.Detached;
            
        return result;
    }

    // UPDATE
    public async Task<bool> TryUpdateAsync(AppTenantInfo tenantInfo)
    {
        dbContext.TenantInfo.Update(tenantInfo);
        var result = await dbContext.SaveChangesAsync() > 0;
        dbContext.Entry(tenantInfo).State = EntityState.Detached;
        return result;
    }

    // REMOVE
    public async Task<bool> TryRemoveAsync(string identifier)
    {
        var existing = await dbContext.TenantInfo
            .Where(ti => ti.Identifier == identifier)
            .SingleOrDefaultAsync();

        if (existing is null)
        {
            return false;
        }

        dbContext.TenantInfo.Remove(existing);
        return await dbContext.SaveChangesAsync() > 0;
    }

    // GET BY IDENTIFIER
    public async Task<AppTenantInfo?> TryGetByIdentifierAsync(string identifier)
    {
        return await dbContext.TenantInfo.AsNoTracking()
            .Where(ti => ti.Identifier == identifier)
            .SingleOrDefaultAsync();
    }

    // GET
    public async Task<AppTenantInfo?> TryGetAsync(string id)
    {
        return await dbContext.TenantInfo.AsNoTracking()
            .Where(ti => ti.Id == id)
            .SingleOrDefaultAsync();
    }

    public async Task<IEnumerable<AppTenantInfo>> GetAllAsync()
    {
        return await dbContext.TenantInfo.AsNoTracking().ToListAsync();
    }
}