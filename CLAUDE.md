# MANDATORY RULES — read before every task

## Architecture (professor's pattern)

### Layers
- **EntityLayer**: data carrier classes (`clsFaculty`, `clsAdvisor`, etc.) and DTO/view classes for joined data (`clsStudentInfo`, `clsAdvisorInfo`). Every entity, DTO and view class lives here with the `cls` prefix.
- **DataAccessLayer**: database access only — no business logic.
- **BusinessLogicLayer**: business logic only — no SQL.
- **Web project** (`OgrenciDanismanlikOtomasyonu`): Controllers + Razor Views + wwwroot.

### DAL pattern
One `public static class` per table, e.g. `clsRoleData`:
- `GetXInfoByID(id)` → entity or null
- `AddNewX(entity)` → new id via `SCOPE_IDENTITY()` or -1
- `UpdateX(entity)` → bool (rowsAffected > 0)
- `DeleteX(id)` → bool
- `GetAllX()` → `List<entity>`
- `IsXExist(id)` → bool

Always use SQL parameters, handle DBNull, use `using` blocks, try/catch returning the failure value.

### BLL pattern
One class per table inheriting the entity, e.g. `clsRoleBusiness : clsRole`:
- `public enum enMode { AddNew, Update }`
- `public enMode Mode { get; private set; }`
- Public constructor → AddNew mode, id = -1
- Private constructor(entity) → Update mode
- `public static Find(id)` → object or null
- Private `_AddNewX()` and `_UpdateX()`
- `public bool Save()` switching on Mode (after a successful add, Mode becomes Update)
- Static `GetAllX()`, `DeleteX(id)`, `IsXExist(id)` delegating to DAL

### Lookup tables
Tables that never change from the site (e.g. Roles, Semesters, RequestPeriods) get **only the read methods**:
- `GetInfoByID` / `Find`, `GetAll`, `IsExist`

**Semesters and RequestPeriods are read-only lookup data loaded from DatabaseScripts (official Selçuk academic calendar). The UI never adds/edits/deletes them. Surveys keep their own dates set by the department.**

### Web pattern
- One Controller per table
- `Index`: shows the list
- `Edit(int? id)` GET: no id → new object, otherwise `Find`, null → `NotFound()`
- `Edit` POST: validate, fill the object, call `Save()`, show success/error with `TempData`
- UI text in **Turkish**

## Naming

Every class name starts with `cls`. File name = class name.

Use clear, meaningful names everywhere — in C#, Razor, JavaScript, CSS classes, ViewData keys, **and SQL table aliases**. Names must be full words but not excessively long (`department`, `studentList`, `facultyId`). Only loop counters may be `i`/`j`.

**Forbidden examples:** `d`, `r`, `x`, `sl`, `SB`, `rp`, `ua`, `rt`, `s`, `a` — and any other single-letter or two-letter abbreviation.

**SQL aliases:** always write `FROM RequestPeriods AS RequestPeriod`, `JOIN Users AS AdvisorUser`, never `FROM RequestPeriods rp` or `JOIN Users ua`.

## Readability

Write in the professor's clear style:
- Braces on their own lines.
- No several statements squeezed onto one line.
- No cryptic abbreviations or prefixes.

## Layout

Every page (existing and future) uses the shared layout (`_Layout.cshtml`) with the sidebar and a centered content column limited by `--content-max-width`. Never use full-width content in the page area.

## Design tokens

All colors, radii, shadows, spacing, fonts, and widths live **only** in the `:root` block of `OgrenciDanismanlikOtomasyonu/wwwroot/css/site.css` as CSS variables. Never hard-code any of these values anywhere else (views, inline styles, other CSS). Bootstrap colors must map to these variables.

Reuse existing partials for UI patterns: `_PageHeader`, `_StatCard`, `_EmptyState`, `_SearchBox`, etc.

## File placement

All web files (Controllers, Views, wwwroot) live inside the `OgrenciDanismanlikOtomasyonu` project folder, never at the solution root. DTO/view classes live in EntityLayer.

## Stack

ASP.NET Core MVC (.NET 10) + SQL Server, ADO.NET with `Microsoft.Data.SqlClient` only. No Entity Framework.

## Connection string

`DataAccessLayer.clsDataAccessSettings.ConnectionString` (set in Program.cs).

## Database

Never change the database schema. Run `.sql` files with `sqlcmd -f 65001` (UTF-8 code page). All Turkish string literals must use the `N'...'` prefix. After inserting Turkish data, verify with `UNICODE()` (e.g. ü = 252, Ö = 214, İ = 304).

## Mobile

Every page must work on phones (400px wide): no hidden or cut-off content. Tables use the responsive stacked-card pattern — below 768px each row becomes a card; every cell shows its column header as a label via `data-label`. Add `data-label="<column header>"` to every `<td>` in every table, and `class="cell-actions"` to the actions column `<td>`. Action buttons are hidden from the label and get larger tap targets automatically via CSS.

## Never

- Never run git commands.
- Never change the database schema.
