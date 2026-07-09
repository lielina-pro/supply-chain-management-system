# Supply Chain Management System — Project Scaffold

**Stack:** ASP.NET Core 8 · C# · SQL Server · Entity Framework Core · JWT RBAC
**Team:** Bethel & Lielina | **Demo every Thursday**
**Schema source of truth:** the team ER diagram (dbdiagram.io) — int identity PKs,
shared StatusTypes/StatusHistory pattern, immutable StockTransactions ledger, BR-12–16.

---

## What actually works right now (Week 3 — Supplier module, FR-02.1, 02.2, 02.4, 02.5)

- Full Supplier registration, profile read/update, and Active/Inactive status
  management, end to end: `SCM.Domain` entities → `SCM.Infrastructure` EF Core
  repositories → `SCM.Application.Suppliers.SupplierService` → both
  `SCM.API.SuppliersController` (REST) and `SCM.Web.SupplierController` (MVC views).
- Registering a supplier creates a linked `User` row, assigns the `Supplier`
  role (BR-09), and writes an initial `StatusHistory` entry.
- Status changes (FR-02.4) go through `StatusTypes`/`StatusHistory`, not a
  hardcoded enum — this is what BR-05 (only active suppliers receive POs)
  will read from once Procurement is built.
- `DbSeeder` seeds the 8 roles and the Supplier `Active`/`Inactive` status
  rows on startup (dev only) — required for the module to function.

**Everything else (Auth, Inventory, Orders, Warehouse, Logistics, Payments,
Notifications, Forecasting, Reporting) is still placeholder interfaces only** —
enough for the solution to compile as a whole, nothing implemented. Those are
later weeks per the 12-week plan.

## Known simplifications vs. the full ER (flag before Week 11 security/BR audit)

- `ArchivedByUserId`, `VerifiedByUserId`, `CreatedBy`-style FK columns are
  plain `int/int?` columns, not EF-enforced foreign keys (no navigation
  property) — acceptable for now, tighten later if we want DB-level FK
  constraints on them.
- `StatusHistory.EntityType/EntityId` is a soft link (no FK), exactly as the
  ER diagram specifies — enforced only in the service layer.
- The MVC (`SCM.Web`) project talks to `SCM.Application` services directly
  in-process rather than calling `SCM.API` over HTTP. This still respects
  the layered architecture (Controllers → Services → Repos → EF) but means
  Web and API each need their own DI wiring in `Program.cs`. Revisit if the
  team wants strict separation later.
- MVC actions aren't `[Authorize]`-protected yet; that depends on Bethel's
  Week 3 auth work landing first (cookie or JWT scheme for `SCM.Web`).

---

## Solution structure

```
SCM/
├── SCM.sln
├── src/
│   ├── SCM.API/                    ← ASP.NET Core 8 Web API
│   │   ├── Controllers/            ← One controller per FR module
│   │   ├── Middleware/             ← Exception handling
│   │   ├── Program.cs              ← DI, JWT auth, RBAC policies, seeding
│   │   └── appsettings.json
│   │
│   ├── SCM.Application/            ← Business use cases (no EF, no HTTP)
│   │   ├── Common/                 ← ServiceResult<T>, exceptions, interfaces
│   │   ├── Suppliers/              ← FR-02 — IMPLEMENTED
│   │   ├── Auth/ Inventory/ Orders/← placeholder interfaces only (later weeks)
│   │   └── ...
│   │
│   ├── SCM.Domain/                 ← Entities, interfaces (int identity PKs)
│   │   ├── Common/                 ← BaseEntity, IArchivable
│   │   ├── Entities/                ← User, Role, UserRole, StatusType,
│   │   │                              StatusHistory, AuditLog, Supplier
│   │   └── Interfaces/Repositories/
│   │
│   └── SCM.Infrastructure/
│       ├── Persistence/            ← ScmDbContext, configs, repos, UnitOfWork, seed
│       └── Identity/                ← BCrypt PasswordHasher
│
└── SCM.Web/                        ← ASP.NET MVC (Suppliers views implemented)
    ├── Controllers/SupplierController.cs
    ├── Views/Supplier/             ← Index, Details, Register, Edit
    ├── ViewModels/
    └── wwwroot/css/site.css
```

---

## Getting started

```bash
# 1. Restore
cd SCM
dotnet restore

# 2. Update connection string if needed
#    src/SCM.API/appsettings.json and SCM.Web/appsettings.json

# 3. Create the initial migration (from SCM.Infrastructure, targeting SCM.API)
cd src/SCM.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../SCM.API
dotnet ef database update --startup-project ../SCM.API

# 4. Run the API
cd ../SCM.API && dotnet run
# Swagger UI: https://localhost:5000/swagger

# 5. Run the MVC site (separate terminal) — also auto-migrates/seeds in dev
cd ../../SCM.Web && dotnet run
```

Default seeded admin (dev only, change immediately): `admin@scm.local` / `ChangeMe123!`

---

## RBAC policies (Program.cs, SCM.API)

| Policy              | Roles allowed                          |
|---------------------|-----------------------------------------|
| `AdminOnly`         | Administrator                          |
| `ProcurementAccess` | Administrator, ProcurementManager      |
| `WarehouseAccess`   | Administrator, WarehouseManager        |
| `LogisticsAccess`   | Administrator, LogisticsCoordinator    |
| `SalesAccess`       | Administrator, SalesManager            |
| `FinanceAccess`     | Administrator, FinanceAnalyst          |
| `SupplierPortal`    | Administrator, Supplier                |
| `CustomerPortal`    | Administrator, Customer                |

---

## Business rules enforced so far

| Rule | Where enforced |
|------|----------------|
| BR-05 Only active suppliers get POs | `Supplier.StatusId` via `StatusTypes` (gate itself will live in Procurement, not yet built) |
| BR-09 Every user has a role | `SupplierService.RegisterAsync` creates a `UserRole` row atomically with the `User` |
| BR-10 Critical ops auditable | `StatusHistory` row written on every supplier status change; `AuditLogs` table modeled, not yet wired into every service |

All other BR-01…BR-16 rules apply to modules not yet built.
