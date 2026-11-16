# UI Redesign Plan - Holidays.Presentation

Visual refresh of the Angular UI. Presentation only: no API, routing, service or validation changes. Functional plan and decisions stay in `docs/FRONTEND_PLAN.md`.

## Status (2026-11-13)

**Done, dark theme.** First built as a light theme, then switched to dark-only the same day at the user's request (colors only, layout unchanged). All 7 steps implemented and checked with screenshots (desktop 1280 px, phone 390 px, and the interactive states). `npx ng build` passes; the only warning is the existing bundle budget one (884 kB). Prettier clean.

## Next steps

1. User: one pass through the create / edit / delete flows in the browser (see "Not verified" below).
2. User: commit. `public/favicon.ico` is deleted (replaced by `favicon.svg`); `src/_theme-colors.scss` and `src/app/shared/empty-state/` are new.

## Critical decisions

| Decision | Choice | Why |
|---|---|---|
| Theme mode | **Dark only**, navy surfaces (user decision 2026-11-13, replacing the light theme built first) | User preference after seeing the light version. No light mode, no toggle. |
| Palette source | Material's own schematic: `npx ng generate @angular/material:theme-color` with a navy seed | Ships with the installed `@angular/material` 22.2.7, so no new dependency. Gives a full, consistent M3 tonal palette. |
| Navy surfaces | `mat.theme-overrides` replaces Material's `surface*`, `on-surface*` and `outline*` values; `--app-*` variables cover the page background, toolbar and accent | Material's dark surfaces are neutral greys. Overriding the system variables makes every component (menus, datepicker, dialog) navy without touching each one. |
| Component styling | `mat.theme-overrides` / component `*-overrides` mixins and `--mat-sys-*` variables. No `::ng-deep`, no selectors into Material internals | Survives Material upgrades; this is the supported API. |
| Shared styles | Global classes in `src/styles.scss` (cards, page header, table, empty state, badges) | The three CRUD pairs share the same look; keeps each component under the 4 kB style budget. |
| Navigation | Keep the top toolbar | Four links. A sidenav is overkill. |
| Font | Keep Roboto (add weight 700 to the existing Google Fonts link) | Already loaded; a new font adds nothing the demo needs. |
| New npm dependencies | None | Everything needed (`MatTooltip`, `MatCard`) is already in `@angular/material`. |
| Verification tooling | Headless Flatpak Chrome, no plugin: `--screenshot` for static pages, DevTools protocol from a throwaway Node script for interactive states | Works with Node 24's built-in `WebSocket`, zero dependencies. Details under "Verification". |
| Cards | Plain `.surface-card` class instead of `MatCard` | Same look with no extra imports in 7 components. |
| Form fields | Outlined appearance, set once with `MAT_FORM_FIELD_DEFAULT_OPTIONS` in `app.config.ts` | Reads better on white cards than the filled default. |
| Confirm dialog | New optional `destructive` flag on `ConfirmDialogData`; the three deletes set it | The dialog stays generic; only deletes get the red button. |

## Palette (as built, in `src/styles.scss`)

| Role | Value | Used for |
|---|---|---|
| Page background `--app-page-background` | `#0A1428` | Behind the cards |
| Toolbar `--app-toolbar-background` | `#0E2048` | Top bar |
| Surface `--mat-sys-surface` | `#111F3B` | Cards, tables, forms |
| Surface containers (low to highest) | `#152543` `#192B4C` `#1E3257` `#243A63` | Table header and row hover, menus, datepicker, dialog |
| Text `--mat-sys-on-surface` / `-variant` | `#E6EBF5` / `#B4C0D8` | Body text / secondary text |
| Borders `--mat-sys-outline-variant` | `#2A3D63` | Card and row borders |
| Primary (generated, tone 80) | `#B2C5FF` | Buttons, links, focus |
| Primary container (generated) | `#22438A` | Badges, success snackbar |
| Accent `--app-accent` (+ `--app-accent-container` `#3B2C08`) | `#F2A900` | "Es festivo" banner, highlighted row in the year table, brand icon |
| Error (generated) | `#FFB4AB`, container `#93000A` | Delete actions, field errors / error snackbar |

## Steps (all done)

1. **Theme foundation** (`src/styles.scss`, generated `src/_theme-colors.scss`, `src/index.html`)
   - Generate the palette, wire it into `mat.theme(...)`, add the `--app-*` variables.
   - Page background, heading scale, link and focus styles.
   - Global helpers: `.page-header` (title + subtitle + action), `.surface-card`, `.data-table` (header tint, row hover, muted id column), `.badge`, `.entity-form`.
2. **Shell** (`app.html`, `app.scss`, `app.ts`)
   - Navy toolbar, brand mark, nav links with icons and a clear active state.
   - Narrow screens: links collapse to icons, content padding shrinks.
3. **Shared pieces** (`shared/`)
   - New `EmptyState` component (icon, message, optional action) to replace the bare `<p>` in the 4 pages that have one.
   - Confirm dialog: destructive button in error color.
   - `Notifier`: distinct success / error snackbar styles via `panelClass`.
4. **List pages** (countries, types, holidays)
   - Table inside a card, tooltips on edit/delete, delete icon in error color.
   - Holidays: type shown as a badge, country filter aligned in the header row.
5. **Form pages** (countries, types, holidays)
   - Form inside a card with a back link; consistent action row.
   - Country form: "Festivos asociados" as a styled list instead of a raw `<ul>`.
6. **Validation page** (home, the one shown first in the demo)
   - Search form in a card.
   - Result banner: amber accent + icon when it is a holiday, neutral when not.
   - Year table: holiday count in the heading, selected date row highlighted.
7. **Finish**
   - Navy calendar SVG favicon replacing the Angular default.
   - `npx ng build` (no new warnings), Prettier, screenshots, update this file, `FRONTEND_PLAN.md` (structure) and `PROJECT_STATUS.md`.

## Shared classes (in `src/styles.scss`)

`.page-header`, `.page-subtitle`, `.back-link`, `.surface-card`, `.table-card`, `.data-table`, `.selected-row`, `.id-cell`, `.actions-cell`, `.danger-action`, `.danger-button`, `.badge`, `.entity-form`, `.form-actions`, `.related-list`, `.snackbar-error`.

## Out of scope

- Dark mode / theme toggle.
- Lazy-loaded routes for the bundle budget (backlog item 12 in `PROJECT_STATUS.md`).
- A generic list/form component (decision in `FRONTEND_PLAN.md` stands).
- Any behavior change: validators, error messages, API calls, routes.

## Verification

Checked by Claude on 2026-11-13 against the running API:

- Light version: every route at 1280 px, three at 390 px, plus the interactive states below.
- Dark version (re-checked after the switch): `/paises`, `/festivos`, `/paises/1` at 1280 px, `/festivos` at 390 px, and all the interactive states below.
- Interactive: select panel, datepicker, result banner for a holiday (Colombia 2026-11-16) and a non-holiday (2026-11-13), highlighted row, form validation errors, delete dialog, error snackbar (409 on Colombia), empty state (holidays filtered by Argentina).

How to repeat it (`npm start` + API running):

```bash
# static page
flatpak run --filesystem=<out-dir> com.google.Chrome --headless=new --window-size=1280,900 \
  --virtual-time-budget=6000 --screenshot=<out-dir>/paises.png http://localhost:4200/paises
# interactive: start Chrome with --remote-debugging-port=<port>, then drive it over the
# DevTools protocol (Page.navigate, Runtime.evaluate, Page.captureScreenshot)
```

**Not verified** (templates changed, logic untouched):

- [ ] Create, edit and delete that actually succeed, and the success snackbar (navy).
- [ ] Tooltips on the edit/delete icons (hover).
- [ ] Type form (`/tipos/:id`); it is the same template as the country form.
- [ ] API down -> error snackbar.

## Risks

| Risk | Mitigation |
|---|---|
| Deadline (2026-11-17) | Presentation-only diff on top of a feature-complete tree; easy to drop if anything looks off. |
| A Material component still showing a grey surface | All `surface-container-*` values are overridden; select panel, datepicker and dialog were checked. |
| Restyle breaks a state nobody looked at | The manual checklist above covers every interactive state. |
| Component style budget (4 kB warning) | Shared rules live in `styles.scss`. |

## Lessons learned

- Flatpak Chrome can take headless screenshots, but only writes to directories granted with `flatpak run --filesystem=<dir>`.
- Material derives `primary` from a tone of the seed palette, not the seed itself: tone 40 in light themes (`#3D5CA4` from seed `#14387F`), tone 80 in dark (`#B2C5FF`). Override the `--mat-sys-*` value with `mat.theme-overrides` when the exact brand color matters.
- Because every color went through `--mat-sys-*` / `--app-*` variables, switching light -> dark was a change to one block of `styles.scss` plus four lines elsewhere.
- "Dark blue palette" was ambiguous (brand color vs dark mode). Asked, got "light", and the user changed their mind on seeing it. For visual work, show a quick screenshot of each option instead of describing it.
- Global classes lose to Material's component styles at equal specificity, because component styles are injected after `styles.css`. `.actions-cell { text-align: right }` never applied until it became `td.actions-cell`.
- Material component looks should be changed with the `mat.<component>-overrides` mixins scoped to a class (`.danger-button`, `.snackbar-error`). An unknown token name fails the Sass build, so typos are caught early.
- `ng generate @angular/material:theme-color --directory=src` wrote `src_theme-colors.scss` at the project root (missing slash); pass `--directory=src/`.
- `pkill -f <pattern>` also matches the shell running it. Use a bracket pattern (`port=933[3]`).
