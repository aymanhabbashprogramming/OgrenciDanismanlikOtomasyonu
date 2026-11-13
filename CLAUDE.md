# PROJECT RULES

## Stack
- ASP.NET Core MVC (.NET 10) + SQL Server, ADO.NET with Microsoft.Data.SqlClient only. No Entity Framework.

## Layers
- **EntityLayer**: data carrier classes (done, do not change)
- **DataAccessLayer**: database access
- **BusinessLogicLayer**: business logic
- **Web project**: Controllers + Razor Views + Bootstrap

## Naming
- Every class name starts with `cls`. File name = class name.

## Connection string
- `DataAccessLayer.clsDataAccessSettings.ConnectionString` (set in Program.cs)

## DAL pattern
One public static class per table, e.g. `clsRoleData`:
- `GetXInfoByID(id)` → entity or null
- `AddNewX(entity)` → new id via `SCOPE_IDENTITY()` or -1
- `UpdateX(entity)` → bool (rowsAffected > 0)
- `DeleteX(id)` → bool
- `GetAllX()` → `List<entity>`
- `IsXExist(id)` → bool

Always use SQL parameters, handle DBNull, use `using` blocks, try/catch returning the failure value.

## BLL pattern
One class per table inheriting the entity, e.g. `clsRoleBusiness : clsRole`:
- `public enum enMode { AddNew, Update }`
- `public enMode Mode`
- Public constructor → AddNew mode, id = -1
- Private constructor(entity) → Update mode
- `public static Find(id)` → object or null
- Private `_AddNewX()` and `_UpdateX()`
- `public bool Save()` switching on Mode (after a successful add, Mode becomes Update)
- Static `GetAllX()`, `DeleteX(id)`, `IsXExist(id)` passing through to DAL

## Lookup tables
Tables that never change from the site (e.g. Roles) get **only the read methods**:
- `GetInfoByID` / `Find`, `GetAll`, `IsExist`

## Web pattern
- One Controller per table
- `Index`: shows the list
- `Edit(int? id)` GET: no id = new object, otherwise `Find`, null → `NotFound()`
- `Edit` POST: validate, fill the object, call `Save()`, show success/error with `TempData`
- UI text in **Turkish**

## Rules
- Never run git commands
- Never change the database schema
