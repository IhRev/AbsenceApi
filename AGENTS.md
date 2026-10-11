# AbsenceApi

ASP.NET Core 9 REST API for organization absence tracking: users, orgs, memberships/invitations, absences, holidays, and admin approval of absence changes.

This file applies to the whole repo. There are no nested `AGENTS.md` files.

## Layout

| Project | Role |
|---|---|
| `src/Absence.Domain` | Entities, `AbsenceEventType`, `IIdKeyed`. No persistence. `UserEntity` extends `IdentityUser`, so this project references `Microsoft.Extensions.Identity.Stores`. |
| `src/Absence.Infrastructure` | EF Core (`AbsenceContext`), SQL Server, Identity, JWT |
| `src/Absence.Api` | Host, feature folders, MediatR handlers, shared results and services |

Solution: `AbsenceApi.sln`. Tests: `tests/Absence.Api.IntegrationTests`.

References point inward: Api → Infrastructure → Domain, and Api also references Domain because handlers use the entity types. Do not reverse those references. Do not reference Api from Infrastructure or Domain.

Feature folders are navigation. A folder holds one controller and one file per use case. Handlers still inject `AbsenceContext` and the shared services in `Common/`.

## Commands

```bash
dotnet restore AbsenceApi.sln
dotnet build AbsenceApi.sln
dotnet test AbsenceApi.sln
dotnet run --project src/Absence.Api/Absence.Api.csproj --launch-profile http
```

HTTP listen URL in the `http` profile: `http://0.0.0.0:5081`. Call it at `http://localhost:5081`. In Development, Swagger UI is at `/`.

EF Core tools manifest: `.config/dotnet-tools.json` (`dotnet-ef` 9.0.19). The tools package is on the Api project.

```bash
dotnet tool restore
dotnet ef migrations add <Name> --project src/Absence.Infrastructure --startup-project src/Absence.Api
dotnet ef database update --project src/Absence.Infrastructure --startup-project src/Absence.Api
```

Integration tests use `WebApplicationFactory<Program>` against SQL Server database `AbsenceDB_Tests`. `Program` is `public partial` for that factory. Pure rules (`AbsenceDateOrder`, `AbsenceHolidayOverlap`) are unit-tested in the same project. Do not mock `IQueryable` or `DbSet`.

Docker: `src/Absence.Api/Dockerfile` (multi-stage, `net9.0`, ports 8080/8081). Restore copies the Api, Domain, and Infrastructure project files.

## Configuration

- Connection string name: `AbsenceDB` (SQL Server).
- JWT: `JwtConfiguration` (`Issuer`, `Audience`, `Secret`, `JwtTokenExpireTimeInMinutes`, `RefreshTokenExpireTimeInDays`). Bound with `ValidateOnStart`.
- Prefer user secrets / env vars for real credentials. Do not commit secrets. Do not copy values out of `appsettings*.json` into docs or commits.

Identity is registered with `AddIdentityCore<UserEntity>` (no Identity UI). Password policy: min length 8; digit/upper/lower/non-alphanumeric are not required. `Stores.MaxLengthForKeys` stays 128, matching the existing Identity tables.

## Architecture

One use case is one file: request DTO (if any) plus a static class holding a `Command` or `Query` record and an `internal` handler. The feature controller lives in the same folder.

```
src/Absence.Api/Features/<Feature>/
  <Feature>Controller.cs
  AddX.cs              // request DTO + static class AddX { Command, Handler }
  GetX.cs              // static class GetX { Query, Handler }
  SharedDto.cs         // only when several use cases share a response type
```

`Command` and `Query` are sealed records. Handlers are `internal` classes. Do not add Commands/, Queries/, Handlers/, DTOs/, or Mappings/ subfolders. Controllers stay in the feature folder, not in `Controllers/`.

Existing features: `Users`, `Organizations`, `Invitations`, `Absences`, `AbsenceTypes`, `Holidays`.

Handlers inject `AbsenceContext` directly. Do not add a repository abstraction or a mapper library. Map entities to DTOs in the handler (`Select` for reads, object initializers for writes).

MediatR pipeline, outer first: `LoggingBehavior` (request type and elapsed time, never the body), then `ValidationBehavior`. Validators are internal FluentValidation classes scanned with `includeInternalTypes: true`. A validation failure becomes `BadRequest` when the request's `OneOf` has that member. Date order for create and update lives in `AbsenceDateOrder` and those command validators. Accepting a stored absence event checks the same helper inside the handler, because those dates are on the saved event.

Shared application services live in `Absence.Api/Common/Services`: `CurrentUser`, `OrganizationAccess`, `AbsenceHolidayOverlapChecker`, `GlobalExceptionHandler`.

### Adding an endpoint

1. Add `Features/<Feature>/<UseCase>.cs` with the request DTO (if any) and `public static class <UseCase>` containing a `Command` or `Query` record plus `internal Handler`.
2. Put shared response DTOs next to use cases in the feature folder.
3. Add a thin action on the feature controller: `Send(new <UseCase>.Command(...))` then map the result to HTTP.

Controllers: `[ApiController]`, primary constructor with `ISender`, `[Authorize]` except `auth/login`, `auth/register`, `auth/refresh_token`. Routes are lowercase; multi-word segments use snake_case (`refresh_token`, `change_password`). Some absence and holiday GETs use absolute routes such as `/organizations/{organizationId}/absences`. Do not rewrite endpoints as Minimal APIs. JSON bodies need `Content-Type: application/json`.

`GET /organizations/{organizationId}/absences` is the caller's own list. The same GET with one or more `userIds` query values is the admin search.

### Results

Handlers return `OneOf<...>` using `OneOf.Types.Success` / `Success<T>` / `NotFound` plus `BadRequest`, `AccessDenied`, `Unauthorized`, and for absence writes `Persisted` and `Queued`. Controllers map with `.Match<ActionResult>`:

- `Success` / `Success<T>` → `Ok`
- `BadRequest` → `BadRequest(message)`
- `NotFound` → `NotFound()`
- `AccessDenied` → `Forbid()`
- `Unauthorized` → `Unauthorized()`
- `Persisted` on create → `Created` (201) with the absence id; on update → `Ok` (200); on delete → `NoContent` (204)
- `Queued` → `Accepted` (202), empty body

Login failure is `BadRequest(message)`. Refresh-token failure is an authentication failure on an anonymous endpoint, so that `BadRequest` result is mapped to `Unauthorized(message)`.

Do not throw for expected business failures. Unexpected exceptions go through `GlobalExceptionHandler` as ProblemDetails (500, or 401 for `MissingUserClaimException`).

### Access

`IOrganizationAccess` is the membership gate. A non-member gets `NotFound`. A member who lacks the right gets `AccessDenied`. `RequireOwnerAsync` is owner-only. On an existing absence, check membership before ownership so a non-member cannot tell a real absence from a missing one.

### Persistence

- Entities implement `IIdKeyed<TId>` and live in `Absence.Domain` (namespace `Absence.Domain`).
- Identity `Id` is `string`. Org and absence foreign keys use `int ShortId`. JWT puts ShortId in claim `shortid`. Inject `IUser` (`Id` vs `ShortId`).
- Handlers query `AbsenceContext` `DbSet`s with LINQ.
- EF configs inherit `EntityConfiguration<TEntity, TId>` and live in `Infrastructure/Database/Configurations`. Migrations live in `Infrastructure/Database/Migrations`. After a model change, add a migration. Do not edit the snapshot by hand. Snapshot entity names are `Absence.Domain.*`; a namespace edit there would make the next migration drop and recreate tables.
- A holiday overlaps an absence when its timestamp is inside the closed range, including both ends (`AbsenceHolidayOverlap.Contains`). The checker inlines that comparison so EF can translate it. Absence list queries use a separate half-open window (`start < windowEnd && end > windowStart`).

### Absences and types

Org admins persist absences immediately (`Persisted`). Other members create `AbsenceEventEntity` rows (`CREATE` / `UPDATE` / `DELETE`) and an admin accepts or rejects them with `RespondAbsenceEvent.Command` (`Queued` until then). Keep that split.

Absence types belong to one organization. Creating an organization seeds Vacation and Sick leave only. Names are unique per organization, max length 30. Deleting a type is refused while an absence or a pending event still references it.

## Code style

- `net9.0`, nullable enabled, implicit usings, file-scoped namespaces.
- Primary constructors. Use the constructor parameters directly; do not copy them into `private readonly` fields.
- LINQ/EF lambdas commonly use `_` as the entity parameter (`_ => _.Name`).
- MediatR, OneOf, and FluentValidation sit on Api. EF, Identity, and JWT sit on Infrastructure. Domain holds the entity types only.

## Do not

- Add a different architecture (minimal APIs, a repository layer, another mapper) unless asked.
- Put business logic in controllers or EF entities.
- Reference Api from Infrastructure or Domain.
- Store JWT secrets or connection strings in source that will be committed.
