# Holidays API - Frontend Contract

Everything the UI needs to talk to the backend. If something here disagrees with the code, the code wins: fix this file.

## Basics

| Item | Value |
|---|---|
| Base URL (http profile) | `http://localhost:7041` |
| Base URL (https profile) | `https://localhost:7040` (http on 7041 redirects to it) |
| Swagger UI | `/swagger` (Development only) |
| Auth | None |
| CORS | **Not configured.** A browser app on another origin will be blocked. See [Frontend gotchas](#frontend-gotchas). |
| Body format | JSON. Request property names are case-insensitive; responses are **camelCase**. |
| Dates | `DateOnly` serialized as `"yyyy-MM-dd"` (e.g. `"2025-04-18"`). |
| IDs | Positive `int`. Route ids use an `:int` constraint, so a non-numeric id returns `404` (no route match), not `400`. |

Run it: `dotnet run --project Holidays.API --launch-profile http` (needs SQL Server, see `PROJECT_STATUS.md`).

## Resources at a glance

| Resource | Route root | Spanish name in routes |
|---|---|---|
| Country | `/api/paises` | Pais |
| Holiday type | `/api/tipos` | Tipo |
| Holiday (definition/rule) | `/api/festivos` | Festivo |
| Calendar (computed dates) | `/api/calendario` | - |

A **Holiday** is a *rule* (e.g. "Corpus Christi = Easter + 61 days, moved to next Monday"), not a date. Actual dates for a given year come only from the **Calendar** endpoints.

## Endpoint reference

### Countries - `/api/paises`

| Method | Path | Body | Success | Errors |
|---|---|---|---|---|
| GET | `/api/paises` | - | `200` `CountryOverview[]` | - |
| GET | `/api/paises/{id}` | - | `200` `CountryDetails` | `404` |
| POST | `/api/paises` | `CreateCountryRequest` | `201` body = new id (`number`) | `400` |
| PUT | `/api/paises/{id}` | `UpdateCountryRequest` | `204` | `400`, `404` |
| DELETE | `/api/paises/{id}` | - | `204` | `404`, `409` if it still has holidays |

### Holiday types - `/api/tipos`

| Method | Path | Body | Success | Errors |
|---|---|---|---|---|
| GET | `/api/tipos` | - | `200` `HolidayTypeOverview[]` | - |
| GET | `/api/tipos/{id}` | - | `200` `HolidayTypeDetails` | `404` |
| POST | `/api/tipos` | `CreateHolidayTypeRequest` | `201` body = new id | `400` |
| PUT | `/api/tipos/{id}` | `UpdateHolidayTypeRequest` | `204` | `400`, `404` |
| DELETE | `/api/tipos/{id}` | - | `204` | `404`, `409` if it still has holidays |

### Holidays - `/api/festivos`

| Method | Path | Body | Success | Errors |
|---|---|---|---|---|
| GET | `/api/festivos` | - | `200` `HolidayOverview[]` (all countries) | - |
| GET | `/api/festivos/{id}` | - | `200` `HolidayDetails` | `404`, `500` if its country/type row is missing |
| POST | `/api/festivos` | `CreateHolidayRequest` | `201` body = new id | `400`, `409` if `countryId`/`typeId` doesn't exist |
| PUT | `/api/festivos/{id}` | `UpdateHolidayRequest` | `204` | `400`, `404`, `409` (same as POST) |
| DELETE | `/api/festivos/{id}` | - | `204` | `404` |

There is no "holidays by country" filter on this resource. Use `GET /api/paises/{id}` (returns the country's holiday id+name list) or filter `GET /api/festivos` client-side by `countryId`.

### Calendar - `/api/calendario`

| Method | Path | Success | Errors |
|---|---|---|---|
| GET | `/api/calendario/festivos/{countryId}/{year}` | `200` `HolidayDate[]` | `400` year not in 1..9999, `404` country, `409` a holiday uses a type with no calculation rule, `500` a fixed holiday has an impossible date (e.g. Feb 30) |
| GET | `/api/calendario/verificar/{countryId}/{year}/{month}/{day}` | `200` bare JSON boolean: `true` / `false` | `400` invalid date, `404` country, `409` / `500` same as above |

- The list is **not sorted**. Sort by `date` client-side.
- A country with no holidays returns `[]`, not `404`.
- `verificar` compares against the *computed* (moved) date. Example: Colombia 2025 "San José" (Mar 19, Wednesday, type 2) is observed Monday Mar 24, so `verificar/1/2025/3/19` -> `false`, `verificar/1/2025/3/24` -> `true`.

## Data shapes

Written as TypeScript for convenience; the JSON is exactly this.

```ts
// ----- Countries -----
interface CountryOverview { id: number; name: string; }
interface CountryDetails  { id: number; name: string; holidays: RelatedHolidaySummary[]; }
interface CreateCountryRequest { name: string; }   // trimmed, 1..100 chars
interface UpdateCountryRequest { name: string; }   // same rules

// ----- Holiday types -----
interface HolidayTypeOverview { id: number; type: string; }
interface HolidayTypeDetails  { id: number; type: string; holidays: RelatedHolidaySummary[]; }
interface CreateHolidayTypeRequest { type: string; } // trimmed, 1..100 chars
interface UpdateHolidayTypeRequest { type: string; }

// ----- Holidays -----
interface RelatedHolidaySummary { id: number; name: string; }

interface HolidayOverview {
  id: number; name: string;
  day: number; month: number; easterDays: number;
  countryId: number; typeId: number;
}
interface HolidayDetails {
  id: number; name: string;
  day: number; month: number; easterDays: number;
  country: CountryOverview;
  type: HolidayTypeOverview;
}
interface CreateHolidayRequest {   // UpdateHolidayRequest is identical
  name: string;        // trimmed, 1..100 chars
  day: number;         // types 1/2/5: must be a real day of `month` (Feb 29 rejected). Types 3/4: ignored, stored as 0
  month: number;       // types 1/2/5: 1..12. Types 3/4: ignored, stored as 0
  easterDays: number;  // types 3/4: offset from Easter Sunday, any int. Types 1/2/5: ignored, stored as 0
  countryId: number;   // must exist, else 409
  typeId: number;      // must exist, else 409
}

// ----- Calendar -----
interface HolidayDate {
  id: number;       // holiday id
  holiday: string;  // holiday NAME (note: not "name")
  date: string;     // "yyyy-MM-dd"
}
```

Request bodies: every field above is required. A missing/`null` string or a wrong JSON type is rejected by ASP.NET before reaching the service (see the 400 shape below).

## Holiday types and how dates are computed

Date calculation is hardcoded by **type id** (`HolidayCalculationFactory`), not by the type's name. Seed ids:

| id | Type (seed name) | Uses | Rule |
|---|---|---|---|
| 1 | Fijo | `day`, `month` | Exact date. |
| 2 | Ley Puente Festivo | `day`, `month` | Date moved to the next Monday (stays if already Monday). |
| 3 | Basado en Pascua | `easterDays` | Easter Sunday + `easterDays`. |
| 4 | Basado en Pascua y Ley Puente Festivo | `easterDays` | Easter Sunday + `easterDays`, then next Monday. |
| 5 | Ley Puente Festivo Viernes | `day`, `month` | Tue -> previous Mon; Wed/Thu -> next Fri; Sat -> previous Fri; Sun -> next Mon; Mon/Fri unchanged. |
| 6+ | (user-created) | - | **No rule.** Any country with a holiday of this type gets `409` on both calendar endpoints. |

On save, the API normalizes the unused fields: Easter-based types (3, 4) get `day = 0, month = 0`; date-based types (1, 2, 5) get `easterDays = 0`. Whatever the form sends in the unused fields is discarded. Unknown type ids (6+) are validated as date-based.

UI implication: the holiday form should switch fields based on `typeId`: show day/month for 1/2/5, show easterDays for 3/4.

## Errors

Two shapes, both `Content-Type: application/problem+json`. Always read `status`, `title`, `detail`; show `detail` to the user.

**1. Business errors (from services)**

```json
{ "status": 409, "title": "Entities/resources conflict.", "detail": "Country (1) cannot be deleted because it has holidays." }
```

| status | title | Typical cause |
|---|---|---|
| 400 | `Validation failed.` | Empty/too-long name, invalid day/month for a date-based type (e.g. `"Day must be between 1 and 28 for month 2."`), bad year, invalid date in `verificar` |
| 404 | `Not found resource.` | `detail` is `"<Entity> (<id>) was not found."` |
| 409 | `Entities/resources conflict.` | Delete with dependants, unknown `countryId`/`typeId` on holiday save, type with no calculation rule |
| 500 | `Internal unexpected error ocurred.` | Holiday references a missing country/type (data corruption) |

**2. Model-binding errors (automatic, from `[ApiController]`)**: missing field, `null`, wrong JSON type, malformed JSON.

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": { "Name": ["The Name field is required."] },
  "traceId": "00-..."
}
```

`errors` keys are property names (PascalCase) or JSON paths like `$.day`. Map them to form fields if you want per-field messages.

**3. Unhandled exceptions** -> `500` with `title: "An unexpected error occurred"`, generic `detail`. Nothing actionable for the UI; show a generic message.

## Frontend gotchas

Ordered by how likely they are to bite during UI work.

1. **CORS is not enabled.** Use the Angular CLI dev proxy so the browser sees same-origin: a `proxy.conf.json` mapping `/api` to `http://localhost:7041`, wired via `ng serve --proxy-config` (or `angular.json`). Then call relative URLs like `/api/paises`.
2. **Use the `http` profile (port 7041) in dev.** Under the `https` profile, `http://...:7041` redirects to 7040 and the proxy target would need `secure: false`.
3. **Holiday form fields depend on type.** Types 3/4: only `easterDays` matters, send anything (e.g. `0`) for day/month. Types 1/2/5: day/month must form a real date; mirror the API rule in the form (max day per month, no Feb 29) to avoid round-trips. The API is the source of truth either way.
4. *(Fixed 2026-11-12: Easter-based validation and day-vs-month validation. See `PROJECT_STATUS.md`.)*
5. **New holiday types are "labels only."** Creating a type works, but assigning it to a holiday makes that country's calendar return `409`. In the holiday form, only offer type ids 1-5, or warn.
6. **`201 Created` Location header is wrong** (`api/paises/5`, relative without leading `/`, a client resolving it gets `/api/api/paises/5`). Ignore it; use the id in the response body.
7. **Calendar list is unsorted.** Sort by `date`.
8. **Calendar item uses `holiday` for the name**, while every other DTO uses `name`.
9. **Easter dates are wrong in 1954, 1981, 2049, 2076 and every year >= 2100** (simplified Gauss algorithm). If the UI has a year picker, a sane range is 1900-2099; the API itself accepts 1..9999.
10. **No pagination, filtering or search** on any list. Fine for the seed size (21 countries, ~30 holidays).

## Seed data (from `DML_Festivos.sql`)

- Types: ids 1-5 as in the table above.
- Countries: 21, ids 1-21. `1 = COLOMBIA`, `10 = ECUADOR`, rest are other Latin American countries + USA, Canada, Spain.
- Holidays: only **Colombia (1)** and **Ecuador (10)** have holidays. Every other country returns `[]` from the calendar.

## Example calls

```bash
BASE=http://localhost:7041

curl $BASE/api/paises
curl $BASE/api/calendario/festivos/1/2025
curl $BASE/api/calendario/verificar/1/2025/12/25          # true

curl -X POST $BASE/api/paises -H 'Content-Type: application/json' -d '{"name":"Portugal"}'   # 201, body: 22

curl -X POST $BASE/api/festivos -H 'Content-Type: application/json' \
  -d '{"name":"Lunes de Pascua","day":0,"month":0,"easterDays":1,"countryId":1,"typeId":3}'

curl -X POST $BASE/api/festivos -H 'Content-Type: application/json' \
  -d '{"name":"Test","day":30,"month":2,"easterDays":0,"countryId":1,"typeId":1}'   # 400

curl -X DELETE $BASE/api/paises/1                         # 409, has holidays
```
