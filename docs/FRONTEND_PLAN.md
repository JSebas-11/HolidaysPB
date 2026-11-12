# Frontend Plan - Holidays.Presentation (Angular)

Plan for the final 15% of the assignment, due **2026-11-17**. API contract lives in `docs/API.md`; overall status in `docs/PROJECT_STATUS.md`.

## Scope

The assignment says: "aplicativo en Angular CLI que incluye CRUD de cada una de las tablas (TipoFestivo, Festivo y Pais) + Página de Consulta de validación de Festivos". Interpreted as:

| Requirement | What we build |
|---|---|
| CRUD Pais | List + create/edit form + delete |
| CRUD TipoFestivo | List + create/edit form + delete |
| CRUD Festivo | List (with country/type names, not ids) + create/edit form + delete |
| Consulta de validación | Country + date -> "Es festivo / No es festivo", plus that country's holidays for the selected year |

Out of scope: auth, unit tests, pagination, i18n, deployment, the calendar view as a separate page, API changes.

## Decisions

| Decision | Choice | Why |
|---|---|---|
| Angular version | 22.2.x (latest stable, MIT, no known vulns on OSV as of 2026-11-12) | Current release; CLI scaffolds the modern defaults. |
| Node | 24 LTS (Angular 22 needs `^22.22.3`, `^24.15.0` or `>=26`) | Available in Fedora repos. |
| UI library | Angular Material + CDK 22.2.x (MIT, no known vulns) | Chosen by user. Gives table, form fields, selects, datepicker, dialog, snackbar. |
| UI language | Spanish. Code in English. | Matches the assignment and data. |
| Location | `Holidays.Presentation/` at repo root, not in the `.slnx` | One repo to hand in. |
| Cross-origin | Angular dev proxy (`/api` -> `http://localhost:7041`) | No API changes. |
| Component style | Standalone components, signals for state, new control flow (`@if`, `@for`) | What Angular 22 generates by default. New apps are zoneless, so UI state must live in signals to re-render. |
| Forms | Reactive Forms | Stable, typed, explicit validators. Closest to how you'd think about validation in C#. |
| CRUD UX | One pattern repeated 3 times: list page + form page (`/x/nuevo`, `/x/:id`), delete with a confirm dialog | Learn it once on countries, copy it twice. |
| Holiday types in holiday form | Only ids 1-5 offered | Types 6+ have no date rule (API `409` on calendar). Decided 2026-11-12, option 1. |
| Error messages | Spanish messages chosen in the UI by HTTP status / context, shown in a snackbar | API `detail` messages are English. |
| `prebundle: false` in `angular.json` serve options | Required on this machine | The repo path contains `c#`. Vite serves prebundled deps from `/@fs/<absolute path>`; the browser treats `#` as a URL fragment, so every Angular import got `index.html` back (blank page). Disabling prebundling bundles deps into `main.js`. Alternative: rename the `c#` folder. |
| No global Angular CLI | `npx ng ...` / `npm start` inside `Holidays.Presentation` | Global `npm -g` with the system Node needs sudo; the project's local CLI is enough. |
| Route params | `withComponentInputBinding()`: `:id` arrives as a component `input()` | Less boilerplate than reading `ActivatedRoute`. |

## Angular in 10 concepts (C# analogies)

Everything this app needs. Learn them as they show up in the milestones, not upfront.

| Concept | What it is | C# / ASP.NET analogy |
|---|---|---|
| Component | A class (`.ts`) + template (`.html`) + styles (`.scss`). A piece of UI. | A Razor view + its code-behind |
| Standalone component | Declares its own `imports: [...]` (other components, Material modules). No NgModules. | `using` directives per file |
| Service + `inject()` | Injectable class, singleton by default (`providedIn: 'root'`). Pulled in with `inject(CountryApi)`. | DI-registered singleton, constructor injection |
| `HttpClient` | Makes HTTP calls, returns an `Observable<T>`. | `HttpClient` returning `Task<T>`, except lazy: nothing happens until you `subscribe` |
| Interfaces as models | TS `interface` matching the JSON shape. | `record` DTOs |
| Signals | `signal(value)`, read with `x()`, write with `x.set()`. Template re-renders when they change. `computed()` derives values. | Observable property with change notification |
| Template syntax | `{{ value }}`, `[prop]="expr"`, `(event)="handler()"`, `@if`, `@for` | Razor `@Model.X`, `@if`, `@foreach` |
| Router | `app.routes.ts` maps URL -> component. `routerLink` for links, `ActivatedRoute` for params. | Attribute routing + route params |
| Reactive Forms | `FormGroup` of `FormControl`s with validators; form state lives in the class. | Model + DataAnnotations / FluentValidation |
| Angular Material | Prebuilt components: `mat-table`, `mat-form-field`, `mat-select`, `mat-datepicker`, `MatDialog`, `MatSnackBar`. | A component library like MudBlazor |

Naming note: since Angular 20 the CLI drops the `.component`/`.service` suffixes (`ng g c country-list` creates `country-list.ts` with class `CountryList`). We name API services `*-api` (`CountryApi`) so they don't clash with model interfaces (`Country...`).

## Project structure

```
Holidays.Presentation/
  proxy.conf.json                     /api -> http://localhost:7041
  angular.json                        serve.options.proxyConfig = proxy.conf.json
  src/
    styles.scss                       Material theme + a few global rules
    app/
      app.ts / app.html               Shell: toolbar with nav links + <router-outlet>
      app.config.ts                   provideRouter, provideHttpClient, date adapter, locale 'es'
      app.routes.ts
      core/
        models/                       country.ts, holiday-type.ts, holiday.ts, calendar.ts (copied from API.md)
        services/                     country-api.ts, holiday-type-api.ts, holiday-api.ts, calendar-api.ts
        errors/error-messages.ts      HttpErrorResponse -> Spanish message
        validators/holiday-date.ts    day-fits-month validator (mirrors API rule, no Feb 29)
      shared/
        confirm-dialog/               Reusable "¿Seguro que desea eliminar...?" dialog
      features/
        countries/                    country-list/, country-form/
        holiday-types/                holiday-type-list/, holiday-type-form/
        holidays/                     holiday-list/, holiday-form/
        holiday-check/                validation page
```

## Routes

| URL | Component | Notes |
|---|---|---|
| `/` | redirect -> `/validacion` | Validation page as home |
| `/validacion` | `HolidayCheck` | |
| `/paises`, `/paises/nuevo`, `/paises/:id` | `CountryList`, `CountryForm` | Same form for create and edit (`:id` present = edit) |
| `/tipos`, `/tipos/nuevo`, `/tipos/:id` | `HolidayTypeList`, `HolidayTypeForm` | |
| `/festivos`, `/festivos/nuevo`, `/festivos/:id` | `HolidayList`, `HolidayForm` | |
| `**` | redirect -> `/validacion` | |

## Screens

**Shell**: `mat-toolbar` with "Festivos" title and links: Validación, Países, Tipos, Festivos.

**Countries / Types (list)**: `mat-table` (Id, Nombre/Tipo, Acciones: editar, eliminar) + "Nuevo" button. Delete -> confirm dialog -> `DELETE` -> reload. `409` -> "No se puede eliminar: tiene festivos asociados."

**Countries / Types (form)**: one field, required, max 100. On edit, load with `GET /{id}`. Save -> `POST`/`PUT` -> back to list + snackbar "Guardado".

**Holidays (list)**: loads holidays + countries + types in parallel (`forkJoin`), shows names instead of ids. Columns: Nombre, País, Tipo, Regla (`25/12` for types 1/2/5, `Pascua +61 días` for 3/4), Acciones. Optional if time allows: country filter `mat-select` (client-side).

**Holidays (form)**:
- Nombre (required, max 100), País (`mat-select` from `/api/paises`), Tipo (`mat-select` from `/api/tipos`, filtered to ids 1-5).
- Type 1/2/5: show Día + Mes. Validators: month 1-12, day valid for that month, no Feb 29.
- Type 3/4: show "Días desde Pascua" (int, can be negative). Send `day: 0, month: 0`.
- API normalizes unused fields anyway, so the form just hides them.

**Validation page (`/validacion`)**:
- País (`mat-select`) + Fecha (`mat-datepicker`, Spanish locale) + "Verificar".
- Calls `verificar/{country}/{y}/{m}/{d}` and `festivos/{country}/{y}` together.
- Result card: "El 24/03/2025 es festivo en COLOMBIA (San José)" or "... no es festivo".
- Below: table of that year's holidays, sorted by date, matching row highlighted.
- Note for the demo: only Colombia (1) and Ecuador (10) have holidays in the seed.
- Date -> URL: use `getFullYear()`, `getMonth() + 1`, `getDate()` (local time). Never `toISOString()`: it converts to UTC and can shift the day.

## Milestones

Each one ends with something visible in the browser. Day estimates assume ~3-4 h/day.

| # | Milestone | Done when | Target |
|---|---|---|---|
| 0 | **Done 2026-11-12.** Environment: Node 24, SQL Server container up, API running, backend fixes checked with curl | `curl localhost:7041/api/paises` returns the countries | Nov 12-13 |
| 1 | **Done 2026-11-12 (pending your browser check).** Scaffold `Holidays.Presentation`, add Material, proxy, locale `es`, shell + routes with placeholder pages | `npm start`, nav between empty pages, a test call through the proxy works | Nov 13 |
| 2 | Models, `*-api` services, error-message helper, confirm dialog | Services callable from a component | Nov 13 |
| 3 | Countries CRUD (the reference pattern, explained step by step) | Create/edit/delete a country, 409 on Colombia shows Spanish message | Nov 14 |
| 4 | Holiday types CRUD (copy of 3) | Same for types | Nov 14 |
| 5 | Holidays CRUD (selects, conditional fields, date validator) | Create a fixed and an Easter-based holiday, Feb 30 blocked in form | Nov 15 |
| 6 | Validation page | 2025-03-24 Colombia -> festivo (San José moved), 2025-03-19 -> no | Nov 15-16 |
| 7 | Manual test pass, polish, run instructions in `PROJECT_STATUS.md` | Checklist below all green | Nov 16 |
| - | Buffer | | Nov 17 |

## Running it

Two terminals:

```bash
# 1. API (from repo root), SQL Server container must be up
dotnet run --project Holidays.API --launch-profile http

# 2. UI
cd Holidays.Presentation
npm install        # first time / after pulling on another machine
npm start          # http://localhost:4200, proxies /api to :7041
```

Fresh machine (e.g. the Mint laptop): Node >= 24.15 (Mint's apt Node is too old; use nvm or NodeSource), then the commands above. If that machine's repo path also contains `#`, `prebundle: false` already covers it.

## How it was scaffolded (for reference)

```bash
sudo dnf install nodejs24
npx -y @angular/cli@22.2.7 new holidays-presentation --directory Holidays.Presentation \
  --style scss --routing --ssr=false --zoneless --ai-config none --commit=false --package-manager npm --interactive=false
cd Holidays.Presentation
npx ng add @angular/material@22.2.7 --skip-confirmation --defaults
npx ng g c --skip-tests features/countries/country-list   # etc. for each page
```

`npm audit`: 0 vulnerabilities at scaffold time. Generators are configured with `skipTests: true` in `angular.json`.

## Manual test checklist (milestone 7)

- [ ] Countries: list, create "Portugal", edit name, delete it; delete Colombia -> conflict message.
- [ ] Empty name / 101 chars blocked in form.
- [ ] Types: list, create, edit, delete; delete "Fijo" -> conflict message.
- [ ] Holidays: list shows country/type names and rule text.
- [ ] Holidays: create fixed (type 1, 25/12), bridge (type 2), Easter-based (type 3, +1), edit an existing seeded Easter holiday without errors, delete.
- [ ] Holidays: 30/02 and 29/02 blocked in form; type select shows only 5 types.
- [ ] Validation: Colombia 2025-12-25 -> sí; 2025-03-19 -> no; 2025-03-24 -> sí (San José).
- [ ] Validation: Ecuador shows its list; Argentina shows "sin festivos registrados".
- [ ] API down -> "No se pudo conectar con el servidor" instead of a blank page.

## How we'll work

You're new to Angular, so each milestone goes: I write it, then explain the new concepts it uses (mapped to the table above), then you run it and check it in the browser. Milestone 3 gets the most explanation; 4-6 reuse it. Ask to slow down or go deeper on anything you'll need to explain to the professor.

## Risks

| Risk | Mitigation |
|---|---|
| Node/SQL Server not set up yet on this machine | Milestone 0 first; it blocks everything else. |
| Zoneless + signals is new even for people who know older Angular (most tutorials online use zone.js and plain properties) | Stick to the patterns in this repo; if a tutorial's UI doesn't update, that's usually the cause. |
| Datepicker timezone shifting the day | Build URL parts from local date getters (see validation page). |
| Professor creates a type 6+ and assigns it | Form only offers 1-5 (accepted limitation). |
| Laptop (Mint) Node version differs | Use Node 24 there too. |
