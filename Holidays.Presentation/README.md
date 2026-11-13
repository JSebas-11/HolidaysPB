# Holidays.Presentation

Angular 22 + Angular Material UI for the Holidays API. UI text in Spanish.

Screens:

| URL | Screen |
|---|---|
| `/validacion` (home) | Holiday check: country + date -> "Es festivo / No es festivo", plus that year's holidays |
| `/paises` | Countries CRUD |
| `/tipos` | Holiday types CRUD |
| `/festivos` | Holidays CRUD |

## Requirements

- Node.js >= 24.15 (or >= 22.22.3)
- The API running on `http://localhost:7041` (see `docs/PROJECT_STATUS.md`, "How to run")
- No `#` in the folder path: Vite's dev server breaks on it

## Run

```bash
npm install   # first time only
npm start     # http://localhost:4200
```

`npm start` proxies every `/api/*` request to `http://localhost:7041` (`proxy.conf.json`), so the API needs no CORS setup.

## Structure

```
src/app/
  core/models/       Interfaces matching the API JSON (+ HolidayTypeId enum)
  core/services/     One service per API resource (CrudService base for the 3 CRUDs, CalendarService)
  core/errors/       HTTP status -> Spanish message
  core/validators/   dayInMonthValidator
  core/utils/        Month names, local date formatting
  shared/            Notifier (snackbar), Confirmation dialog
  features/          One folder per screen
```

Full API contract: `docs/API.md`. Design decisions: `docs/FRONTEND_PLAN.md`.

## Commands

```bash
npm start                         # dev server
npx ng build                      # production build into dist/
npx prettier --write "src/**/*.{ts,html,scss}"   # format
```
