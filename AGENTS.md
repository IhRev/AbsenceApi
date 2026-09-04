# AbsenceApi

ASP.NET Core 9 REST API for organization absence tracking: users, orgs, memberships/invitations, absences, holidays, and admin approval of absence changes.

This file applies to the whole repo. There are no nested `AGENTS.md` files.

## Layout

| Project | Role |
|---|---|
| `src/Absence.Api` | Host, feature slices (controller + one file per use case), shared results, `IUser` (`CurrentUser`), overlap checker, exception handler |
| `src/Absence.Infrastructure` | EF Core (`AbsenceContext`), entities, SQL Server, Identity, JWT |

Solution: `AbsenceApi.sln`. The `tests` solution folder has **no test projects**.

Layering: Api → Infrastructure. Do not reverse that reference. Feature folders live in Api; persistence and Identity live in Infrastructure.

## Commands

```bash
dotnet restore AbsenceApi.sln
dotnet build AbsenceApi.sln
dotnet run --project src/Absence.Api/Absence.Api.csproj --launch-profile http
```

HTTP listen URL in the `http` profile: `http://0.0.0.0:5081`. In Development, Swagger UI is at `/`.

EF Core (tools package is on the Api project):

```bash
dotnet ef migrations add <Name> --project src/Absence.Infrastructure --startup-project src/Absence.Api
dotnet ef database update --project src/Absence.Infrastructure --startup-project src/Absence.Api
```

There is no test project, linter config, CI workflow, or `Directory.Build.props`. Do not invent `dotnet test` / format pipelines. When tests are added, prefer integration tests for handlers (`WebApplicationFactory` + SQL) and unit tests for pure rules (date order, overlap policy). Do not mock `IQueryable` / `DbSet`.

Docker: `src/Absence.Api/Dockerfile` (multi-stage, `net9.0`, ports 8080/8081).

## Configuration

- Connection string name: `AbsenceDB` (SQL Server).
- JWT: `JwtConfiguration` (`Issuer`, `Audience`, `Secret`, `JwtTokenExpireTimeInMinutes`, `RefreshTokenExpireTimeInDays`).
- Prefer user secrets / env vars for real credentials. Do not commit secrets. Do not copy values out of `appsettings*.json` into docs or commits.

Identity password policy (Infrastructure): min length 8; digit/upper/lower/non-alphanumeric **not** required.

## Architecture

Vertical slices: one use case = one file (request DTO + `Command`/`Query` + `Handler` nested in a static class). The feature controller lives in the same folder.

```
src/Absence.Api/Features/<Feature>/
  <Feature>Controller.cs
  AddX.cs              // request DTO + static class AddX { Command, Handler }
  GetX.cs              // static class GetX { Query, Handler }
  SharedDto.cs         // only when several slices share a response type
```

Do not add Commands/, Queries/, Handlers/, DTOs/, or Mappings/ subfolders. Controllers stay in the feature folder, not in `Controllers/`.

Existing features: `Users`, `Organizations`, `Invitations`, `Absences`, `AbsenceTypes`, `Holidays`.

Handlers inject `AbsenceContext` directly. Do not add generic repository abstractions.

### Adding an endpoint

1. Add `Features/<Feature>/<UseCase>.cs` with the request DTO (if any) and `public static class <UseCase>` containing `Command` or `Query` plus `internal Handler`.
2. Put shared response DTOs next to slices in the feature folder (not a DTOs/ subfolder).
3. Map entities to DTOs in the handler (`Select` for reads, object initializers / property assignment for writes). Do not add a mapper library.
4. Add a thin action on the feature controller: `Send(new <UseCase>.Command(...))` then map the result to HTTP.

Controllers: `[ApiController]`, primary constructor with `ISender`, `[Authorize]` except `auth/login`, `auth/register`, `auth/refresh_token`. Routes are lowercase; multi-word segments use snake_case (`refresh_token`, `change_password`, `event_types`). Some absence/holiday GETs use absolute routes like `/organizations/{organizationId}/absences`. Do not rewrite endpoints as Minimal APIs.

### Results

Handlers return `OneOf<...>` with `OneOf.Types.Success` / `Success<T>` / `NotFound` plus app structs `BadRequest` and `AccessDenied`. Controllers map with `.Match<ActionResult>`:

- `Success` / `Success<T>` → `Ok` (sometimes `Ok(new { Message = ... })`)
- `BadRequest` → `BadRequest(message)`
- `NotFound` → `NotFound()`
- `AccessDenied` → `Forbid()`

Do not throw for expected business failures. Unexpected exceptions go through `GlobalExceptionHandler` as RFC 7231 500 ProblemDetails.

### Persistence

- Entities implement `IIdKeyed<TId>` and live in `Absence.Infrastructure/Entities`.
- `UserEntity` extends `IdentityUser`. Identity `Id` is `string`; org/absence FKs use `int ShortId`. JWT puts ShortId in claim `shortid`. Inject `IUser` in handlers (`Id` vs `ShortId`).
- Handlers query `AbsenceContext` `DbSet`s with LINQ. Shared holiday/absence overlap uses `AbsenceContext` via `IAbsenceHolidayOverlapChecker`.
- Do not add generic `IRepository<T>` or specialized repository wrappers.
- EF configs inherit `EntityConfiguration<TEntity, TId>` and live in `Infrastructure/Database/Configurations`. Migrations in `Infrastructure/Database/Migrations`. After model changes, add a migration; do not edit the snapshot by hand unless fixing a broken migration.

### Absence approval

Org **admins** persist absences immediately. Non-admins create `AbsenceEventEntity` rows (`CREATE` / `UPDATE` / `DELETE`) for admin accept/reject via `RespondAbsenceEventCommand`. Preserve that split when changing absence write paths.

## Code style

- `net9.0`, nullable enabled, implicit usings, file-scoped namespaces.
- Primary constructors; handlers use constructor parameters directly.
- LINQ/EF lambdas commonly use `_` as the entity parameter (`_ => _.Name`).
- Slice `Handler` types are `internal`.
- MediatR and OneOf sit on **Api**. EF, Identity, and JWT sit on **Infrastructure**.

## Do not

- Add a different architecture (minimal APIs, Result monad library, another mapper) unless asked.
- Put business logic in controllers or EF entities.
- Reference Api from Infrastructure.
- Introduce tests, CI, or editorconfig unless requested — none exist today.
- Store JWT secrets or connection strings in source that will be committed.
