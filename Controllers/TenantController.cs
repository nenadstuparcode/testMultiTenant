using Dumpify;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using FinbuckleMultiTenant.Data;
using FinbuckleMultiTenant.Data.Store;
using FinbuckleMultiTenant.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinbuckleMultiTenant.Controllers;

[ApiController]
[Route("[controller]")]
public class TenantController(
    EFCoreStore store, 
    AppDbContext context, 
    IMultiTenantContextSetter multiTenantContextSetter, 
    IConfiguration config, 
    IMultiTenantContextAccessor<AppTenantInfo> accessor) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> GetTenants()
    {
        return Ok(await store.GetAllAsync());
    }
    
    [HttpGet("tenants/{identifier}")]
    public async Task<ActionResult> GetTenantById([FromRoute] string identifier)
    {
        var tenant = await store.TryGetByIdentifierAsync(identifier);
        if(tenant == null) return NotFound();
        
        return Ok(tenant);
    }

    [HttpDelete("tenants/{identifier}")]
    public async Task<ActionResult> DeleteTenant([FromRoute] string identifier)
    {
        var tenant = await store.TryGetByIdentifierAsync(identifier);
        if(tenant == null) return NotFound();

        var removed = await store.TryRemoveAsync(tenant.Identifier);
        
        if(removed) return NoContent();
        
        return BadRequest();
    }
    
    [HttpPost]
    public async Task<ActionResult> CreateTenant(CreateTenantRequest createTenantRequest)
    {
        var defaultConnection = config.GetConnectionString("DefaultConnection")!;

        defaultConnection.Replace("DefaultDB", createTenantRequest.Identifier).Dump();
        var newTenant = new AppTenantInfo()
        {
            Id = Guid.NewGuid().ToString(),
            Name = createTenantRequest.Name,
            Identifier = createTenantRequest.Identifier,
            ConnectionString = defaultConnection.Replace("DefaultDB", createTenantRequest.Identifier)
        };
        
        var userAdded = await store.TryAddAsync(newTenant);

        if(!userAdded) return BadRequest();
        
        var tenantToUse = await store.TryGetByIdentifierAsync(newTenant.Identifier);

        tenantToUse.Dump();

        multiTenantContextSetter.MultiTenantContext = new MultiTenantContext<AppTenantInfo>()
        {
            TenantInfo = tenantToUse
        };

        
        await context.Database.MigrateAsync();
        var connected = await context.Database.CanConnectAsync();
        var created = await context.Database.EnsureCreatedAsync();
        created.Dump();
        connected.Dump();
    
        
        context.Blogs.Add(new Blog()
        {
            Id = Guid.NewGuid(),
            Content = "test",
            Title = "test",
            CreatedAt = DateTime.UtcNow,
        });

        await context.SaveChangesAsync();
  
        
        // if (context.Database.GetMigrations().Any())
        // {
        //     Console.Write("Database migrations are running... GetMigrations");
        //     await context.Database.MigrateAsync();
        //     if ((await context.Database.GetPendingMigrationsAsync()).Any())
        //     {
        //        
        //         Console.Write("Database migrations are running... GetMigrationsAsync");
        //         await context.Database.MigrateAsync();
        //         await context.Database.EnsureCreatedAsync();
        //     }
        // }

        //TODO: Add root/admin user to tenant application
        
   
        
        return Ok(newTenant);
    }
}