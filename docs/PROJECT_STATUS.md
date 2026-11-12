# HolidaysPB - Project Status

College project: REST API that stores holiday rules per country and computes the actual holiday dates for any year. Backend done and approved by the professor; next phase is the UI.

For the HTTP contract (endpoints, DTOs, errors, frontend gotchas) read **`docs/API.md`**. This file covers how the solution is built, how to run it, and where we are.

## Assignment (final 15%, due 2026-11-17)

Angular CLI app with:
- CRUD for every table: TipoFestivo (`/api/tipos`), Festivo (`/api/festivos`), Pais (`/api/paises`).
- Holiday validation page ("is this date a holiday in country X?", `GET /api/calendario/verificar/...`).

Scope is "make it work for the assignment". No extra polish or API work beyond what blocks the UI.

## Current status (2026-11-12)

- Backend: all 5 layers implemented, builds with 0 errors. CRUD for countries, types and holidays + calendar endpoints working.
- Backend fixes done: Easter-based holiday validation and day-vs-month validation (Known issues 1, 2).
- Backend fixes verified live against SQL Server (2026-11-12): Easter types save as 0/0, impossible dates rejected, seeded Easter holiday round-trips with 204.
- Frontend: milestones 0-2 done (scaffold, Material, proxy, Spanish locale, toolbar shell, routes with placeholder pages; models, API services, error messages, notifier, confirm dialog). Next: milestone 3 (Countries CRUD). Plan: `docs/FRONTEND_PLAN.md`.
- Dev machine: Node 24.18, SQL Server in Docker (`sql_server` container).
- Local DB has non-seed test data: country 22 "UNITED KINGDOM" and type 6 "Saint Patrick's Day" with holiday 32 "Tech day". Because of it, the UK calendar returns 409 (expected, see Known issue 3).
- Tests: none.
- Docs: `docs/API.md` (contract), `docs/FRONTEND_PLAN.md` (UI plan + milestones), this file (status).

## Next steps

Follow the milestones in `docs/FRONTEND_PLAN.md`, starting with milestone 0 (Node 24, Angular CLI, SQL Server up, smoke-test the API).

## Open decisions

None blocking. UI decisions (Angular 22, Material, Spanish UI, dev proxy, holiday types limited to 1-5 in the form) are recorded in `docs/FRONTEND_PLAN.md`.

## How to run

1. SQL Server reachable on `localhost,1433` (Docker container in the dev setup).
2. Run `Holidays.Infrastructure/Persitence/scripts/DDL_Festivos.sql`, then `DML_Festivos.sql`. There are **no EF migrations**; the schema comes only from these scripts.
3. Set `ConnectionStrings:DBConnection` (database name `Festivos`). Prefer user-secrets over `appsettings.json`:
   ```bash
   dotnet user-secrets init --project Holidays.API
   dotnet user-secrets set "ConnectionStrings:DBConnection" "Server=localhost,1433;Database=Festivos;User=sa;Password=<pwd>;TrustServerCertificate=True;" --project Holidays.API
   ```
4. `dotnet run --project Holidays.API --launch-profile http` -> `http://localhost:7041/swagger`.

The app throws on startup if `DBConnection` is missing.

## Architecture

.NET 10, ASP.NET Core controllers, EF Core (SQL Server), Swashbuckle. Central package versions in `Directory.Packages.props`.

```
Holidays.Domain          Entities (Country, Holiday, HolidayType) + constants (table/column names, max lengths)
Holidays.Core            Contracts: service interfaces, request/response records, Result/AppError, repository + UoW interfaces
Holidays.Application     Service implementations, validation ("Converters"), mapping ("Mappers"), date calculation strategies
Holidays.Infrastructure  DbContext, repositories, UnitOfWork, SQL scripts
Holidays.API             Controllers, Result -> HTTP mapping, global exception handler, DI composition
```

Dependency direction: `API -> Application -> Core -> Domain`, `Infrastructure -> Core`. API composes everything via `AddHolidaysPB()`.

### Request flow

`Controller` -> `I*Service` (Core) -> `*Service` (Application): `*Converter` validates/normalizes the request into an entity -> repository -> `IUnitOfWork.SaveChangesAsync` -> `*Mapper` builds the response record -> `Result<T>` -> `ToApiResult()` maps success to 200/201/204 and `AppError.Kind` to a ProblemDetails status.

### Where things live

| Need to change... | File |
|---|---|
| Routes / HTTP status on success | `Holidays.API/Controllers/*Controller.cs` |
| ErrorKind -> HTTP status/title | `Holidays.API/Common/Extensions/ResultExtension.cs` |
| Request/response shapes | `Holidays.Core/Services/<Feature>/Requests.cs`, `Responses.cs` |
| Input validation | `Holidays.Application/Features/<Feature>/Utils/*Converter.cs` |
| Date calculation per type | `Holidays.Application/Features/Holidays/Calculation/` (factory + strategies + `HolidayDateHelper`) |
| Max lengths, table/column names | `Holidays.Domain/Constants/` |
| DI / startup | `Holidays.API/Program.cs`, `*/Extensions/DependencyInjection.cs` |

## Critical decisions

- **Result pattern instead of exceptions** for expected failures. Services return `Result`/`Result<T>` with an `AppError(Kind, Message)`; the API maps `Kind` to status codes in one place. Exceptions are reserved for real bugs and caught by `GlobalExceptionHandler` (500).
- **Strategy + factory for date calculation**, keyed by `TypeId` 1-5 (constants in `Calculation/HolidayTypeIds.cs`, shared with validation). Keeps each rule isolated and testable. Trade-off: the mapping is hardcoded to seed ids, so user-created types have no rule.
- **Holiday validation depends on type** (2026-11-12). Easter-based (3, 4): day/month not validated, stored as 0 (matches seed). Date-based (1, 2, 5, and unknown ids): day must exist in the month, `easterDays` stored as 0. **Feb 29 is rejected** on purpose: it can't be computed in non-leap years.
- **No CORS in the API** (2026-11-12). The Angular dev proxy handles cross-origin in dev; keeps API changes to the minimum for the deadline.
- **`CalculationMode` on `HolidayType` was dropped** (commented out in entity, DbContext, constants, seed). The strategy is chosen by type id instead of a stored mode.
- **Contracts in Core, implementations in Application.** API only depends on interfaces + records from Core.
- **Spanish DB schema and routes, English code.** Mapping via `DomainConstants.Database`. Routes match the assignment (`/api/paises`, `/api/festivos`, ...).
- **Schema from SQL scripts, not migrations.** Matches what the course provided.
- **Delete is restricted**: countries/types with holidays return 409 (checked in service + `DeleteBehavior.Restrict` on FKs).
- **Separate tracked vs read-only repo reads** (`GetByIdAsync` tracked for updates/deletes, `GetReadOnlyByIdAsync` with `AsNoTracking` for queries).

## Known issues / backlog

Ordered by impact on the UI.

1. ~~**Easter-based holidays fail validation.**~~ Fixed 2026-11-12 in `HolidayConverter`: types 3/4 skip day/month validation and store `0/0`.
2. ~~**Day not validated against month.**~~ Fixed 2026-11-12: validated with `DateTime.DaysInMonth` on a non-leap year (Feb 29 rejected). Existing bad rows in the DB (if any were inserted before the fix) still break the calendar; the seed has none.
3. **User-created holiday types have no calculation rule** -> 409 on the calendar for any country using them. **Accepted (2026-11-12)**: the UI's holiday form only offers types 1-5; no API change.
4. **Easter algorithm is wrong for 1954, 1981, 2049, 2076 and all years >= 2100** (Gauss shortcut with fixed constants, missing the two exception rules). Verified against the Meeus/Jones/Butcher algorithm. Fix: swap `GetEasterSunday` for the anonymous Gregorian algorithm, or clamp year to 1900-2099.
5. **`Created` Location header is relative** (`api/paises/{id}`, no leading `/`). Use `CreatedAtAction(...)` or prefix `/`.
6. **No CORS.** Not needed while using the Angular dev proxy; needed if the UI is ever served from another origin.
7. **Calendar list unsorted.** Trivial to `OrderBy(date)` server-side.
8. **500 response leaks the exception type name** (`Type = ex.GetType().Name` in `GlobalExceptionHandler`). Minor, but drop it or use a URI.
9. **Vulnerable transitive package**: `System.Security.Cryptography.Pkcs 9.0.11` (one moderate, one high advisory, NU1902/NU1903 on build). Pulled in transitively; pin a patched version in `Directory.Packages.props` (transitive pinning is already enabled).
10. **No tests.** The calculation strategies are the obvious first target.
11. **Stale `.http` file** (template leftover).

## Lessons learned

- Hardcoding behavior to seed ids (`TypeId` 1-5) couples logic to data. It works for a fixed dataset but makes the "create type" endpoint misleading.
- Validation rules have to account for every type's shape: a single day/month rule broke the Easter-based types the seed data already contains.
- Validating fields independently (day, month) isn't enough when the real constraint is the combination (a valid date).
- When several places depend on the same seed ids, name them once (`HolidayTypeIds`) instead of repeating magic numbers.
- A `#` in the project path (`Coding/c#/`) breaks Vite's dev server in the browser (URL fragment). `curl` returned 200 for the broken URLs because of the SPA fallback; checking the `Content-Type` is what exposed it. Fixed by renaming the folder to `Coding/cs/` (temporary workaround was `prebundle: false`).
- Simplified Easter formulas only hold for a limited year range; check edge years against a reference before trusting one.
