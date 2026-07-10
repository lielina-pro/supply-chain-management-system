using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SCM.API.Middleware;
using SCM.Application.Auth.Interfaces;
using SCM.Application.Auth.Services;
using SCM.Application.Common.Interfaces;
using SCM.Application.Suppliers.Interfaces;
using SCM.Application.Suppliers.Services;
using SCM.Domain.Interfaces;
using SCM.Infrastructure.Identity;
using SCM.Infrastructure.Persistence;
using SCM.Infrastructure.Persistence.Seed;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<ScmDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

// Suppliers module (FR-02)
builder.Services.AddScoped<ISupplierService, SupplierService>();

// Auth module (FR-01.1, FR-01.2, FR-01.3) — Week 3: Bethel
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
// JWT
var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = builder.Configuration["Jwt:Issuer"],
            ValidAudience            = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// RBAC policies (FR-01.3)
builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("AdminOnly",         p => p.RequireRole("Administrator"));
    o.AddPolicy("ProcurementAccess", p => p.RequireRole("Administrator","ProcurementManager"));
    o.AddPolicy("WarehouseAccess",   p => p.RequireRole("Administrator","WarehouseManager"));
    o.AddPolicy("LogisticsAccess",   p => p.RequireRole("Administrator","LogisticsCoordinator"));
    o.AddPolicy("SalesAccess",       p => p.RequireRole("Administrator","SalesManager"));
    o.AddPolicy("FinanceAccess",     p => p.RequireRole("Administrator","FinanceAnalyst"));
    o.AddPolicy("SupplierPortal",    p => p.RequireRole("Administrator","Supplier"));
    o.AddPolicy("CustomerPortal",    p => p.RequireRole("Administrator","Customer"));
});

// Localization (NFR-7.7)
builder.Services.AddLocalization(o => o.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(o =>
{
    var supported = new[] { "en", "am" };
    o.SetDefaultCulture("en")
     .AddSupportedCultures(supported)
     .AddSupportedUICultures(supported);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Type         = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme       = "Bearer",
        BearerFormat = "JWT",
        In           = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description  = "Enter your JWT token. Example: eyJhbGci..."
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Auto-migrate + seed roles/status types in dev so the Supplier module
    // works out of the box. In a real deployment this would be a separate
    // release step, not run on every startup.
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ScmDbContext>();
    await DbSeeder.SeedAsync(db);
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseRequestLocalization();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
