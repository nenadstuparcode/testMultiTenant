using Finbuckle.MultiTenant;
using FinbuckleMultiTenant.Data;
using FinbuckleMultiTenant.Data.Store;
using FinbuckleMultiTenant.Models;

namespace FinbuckleMultiTenant;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddScoped<EFCoreStore>();
        builder.Services.AddMultiTenant<AppTenantInfo>()
            .WithHostStrategy()
            // .WithHeaderStrategy()
            .WithEFCoreStore<TenantDbContext, AppTenantInfo>();
        builder.Services.AddDbContext<AppDbContext>();
        
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMultiTenant();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}