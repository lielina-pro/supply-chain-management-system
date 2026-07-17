using Microsoft.EntityFrameworkCore;
using SCM.Application.Common.Interfaces;
using SCM.Application.Suppliers.Interfaces;
using SCM.Application.Suppliers.Services;
using SCM.Application.Procurement.Interfaces;
using SCM.Application.Procurement.Services;
using SCM.Domain.Interfaces;
using SCM.Infrastructure.Identity;
using SCM.Infrastructure.Persistence;
using SCM.Infrastructure.Persistence.Seed;
using SCM.Application.Procurement.Interfaces;
using SCM.Application.Procurement.Services;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ScmDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IPurchaseRequestService, PurchaseRequestService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ScmDbContext>();
    await DbSeeder.SeedAsync(db); // idempotent - safe alongside SCM.API also seeding
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
