# AbsenceApi

ASP.NET Core 9 REST API for organization absence tracking: users, orgs, memberships/invitations, absences, holidays, and admin approval of absence changes.

This file applies to the whole repo. There are no nested `AGENTS.md` files.

## Layout

| Project | Role |
|---|---|
| `src/Absence.Api` | Controllers, JWT/Swagger wiring, `IUser` (`CurrentUser`), exception handler |
| `src/Absence.Application` | MediatR use cases, DTOs, AutoMapper profiles, result types, identity abstractions |
| `src/Absence.Domain` | Entities, repository interfaces, enums |
| `src/Absence.Infrastructure` | EF Core (`AbsenceContext`), SQL Server, Identity, JWT, repository implementations |

Solution: `AbsenceApi.sln`. The `tests` solution folder has **no test projects**.

Layering: Api → Infrastructure → Application → Domain. Do not reverse those references.

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

There is no test project, linter config, CI workflow, or `Directory.Build.props`. Do not invent `dotnet test` / format pipelines.

Docker: `src/Absence.Api/Dockerfile` (multi-stage, `net9.0`, ports 8080/8081).

## Configuration

- Connection string name: `AbsenceDB` (SQL Server).
- JWT: `JwtConfiguration` (`Issuer`, `Audience`, `Secret`, `JwtTokenExpireTimeInMinutes`, `RefreshTokenExpireTimeInDays`).
- Prefer user secrets / env vars for real credentials. Do not commit secrets. Do not copy values out of `appsettings*.json` into docs or commits.

Identity password policy (Infrastructure): min length 8; digit/upper/lower/non-alphanumeric **not** required.

## Architecture

Clean-ish CQRS: controllers inject `ISender` and dispatch commands/queries. Handlers contain business logic.

Use-case folders:

```
src/Absence.Application/UseCases/<Feature>/{Commands,Queries,Handlers,DTOs,Mappings}/
```

Existing features: `Users`, `Organizations`, `Invitations`, `Absences`, `AbsenceTypes`, `AbsenceEventTypes`, `Holidays`.

### Adding an endpoint

1. DTO under the feature `DTOs/` folder.
2. `IRequest<T>` command or query (primary constructor wrapping the DTO / route values).
3. `internal` handler implementing `IRequestHandler<,>`.
4. `internal` AutoMapper `Profile` in `Mappings/` when mapping entities ↔ DTOs.
5. Thin controller method: `Send` then map the result to HTTP.

Controllers: `[ApiController]`, primary constructor with `ISender`, `[Authorize]` except `auth/login`, `auth/register`, `auth/refresh_token`. Routes are lowercase; multi-word segments use snake_case (`refresh_token`, `change_password`, `event_types`). Some absence/holiday GETs use absolute routes like `/organizations/{organizationId}/absences`.

### Results

Handlers return `OneOf<...>` with `OneOf.Types.Success` / `Success<T>` / `NotFound` plus app structs `BadRequest` and `AccessDenied`. Controllers map with `.Match<ActionResult>`:

- `Success` / `Success<T>` → `Ok` (sometimes `Ok(new { Message = ... })`)
- `BadRequest` → `BadRequest(message)`
- `NotFound` → `NotFound()`
- `AccessDenied` → `Forbid()`

Do not throw for expected business failures. Unexpected exceptions go through `GlobalExceptionHandler` as RFC 7231 500 ProblemDetails.

### Domain / persistence

- Entities implement `IIdKeyed<TId>` and live in `Absence.Domain/Entities`.
- `UserEntity` extends `IdentityUser`. Identity `Id` is `string`; org/absence FKs use `int ShortId`. JWT puts ShortId in claim `shortid`. Inject `IUser` in handlers (`Id` vs `ShortId`).
- Generic `IRepository<T>` plus specialized repos (`IOrganizationUsersRepository`, `IAbsenceEventRepository`, `IOrganizationUserInvitationsRepository`). Queries are composed as `Func<IQueryable<T>, IQueryable<T>>[]`.
- EF configs inherit `EntityConfiguration<TEntity, TId>` and live in `Infrastructure/Database/Configurations`. Migrations in `Infrastructure/Database/Migrations`. After model changes, add a migration; do not edit the snapshot by hand unless fixing a broken migration.

### Absence approval

Org **admins** persist absences immediately. Non-admins create `AbsenceEventEntity` rows (`CREATE` / `UPDATE` / `DELETE`) for admin accept/reject via `RespondAbsenceEventCommand`. Preserve that split when changing absence write paths.

## Code style

- `net9.0`, nullable enabled, implicit usings, file-scoped namespaces.
- Primary constructors; handlers still assign to `private readonly` fields.
- LINQ/EF lambdas commonly use `_` as the entity parameter (`_ => _.Name`).
- Handlers and AutoMapper profiles are `internal`.
- MediatR package currently sits on **Domain** (Application uses it transitively). AutoMapper and OneOf are on Application. Follow existing package placement unless you are explicitly fixing references.

## Do not

- Add a different architecture (minimal APIs, Result monad library, another mapper) unless asked.
- Put business logic in controllers or EF entities.
- Reference Infrastructure from Application, or Domain from Api.
- Introduce tests, CI, or editorconfig unless requested — none exist today.
- Store JWT secrets or connection strings in source that will be committed.
