# Horoscope API — Backend Challenge

A REST API where authenticated users get their **daily horoscope** based on the
zodiac sign derived from their birth date, see **how many days are left until their
next birthday**, manage their **profile**, and browse **statistics and history** of
past lookups. Horoscope text comes from an external provider and is **cached** to
avoid redundant calls.

Built with **.NET 10**, ASP.NET Core (controller-based), **EF Core (Code First) +
SQL Server**, **JWT** authentication and **FluentValidation**.

---

## Scenario

Implement the backend for a horoscope service. A user signs up with their birth
date; from it the system computes the zodiac sign and asks an external horoscope
API for the daily text. Every lookup is stored so the API can provide usage
statistics. Repeated lookups for the same sign on the same day must **not** hit
the external API again.

## Tasks implemented

- **Authentication** with JWT (register / login), passwords hashed with BCrypt.
- **Daily horoscope** (`/api/horoscope/today`): computes the sign + days to next
  birthday and returns the horoscope text for the authenticated user.
- **External provider** consumed through a **typed `HttpClient`**.
- **In-memory cache** keyed by `sign + date` so the same request does not call the
  external API twice (covered by a unit test).
- **Profile** management (get / partial update; username is immutable).
- **Statistics**: most searched sign, full ranking, and the user's **paginated**
  history.
- **Query history** persisted in the database.
- Cross-cutting: global exception handling, consistent response envelope,
  FluentValidation via an action filter, Swagger with JWT support, CORS.
- **55 unit tests** (xUnit + Moq).

---

## Tech stack

| Area | Choice |
| --- | --- |
| Runtime | .NET 10 |
| Web | ASP.NET Core (controllers) |
| Data | EF Core 10 (Code First) + SQL Server |
| Auth | JWT Bearer + BCrypt.Net-Next |
| Validation | FluentValidation |
| Caching | Microsoft.Extensions.Caching.Memory |
| Docs | Swagger / Swashbuckle |
| Tests | xUnit + Moq |

---

## Getting started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (any edition: Express, Developer, LocalDB, etc.)
- Optional (for the EF route): `dotnet tool install --global dotnet-ef`

### 1. Clone

```bash
git clone https://github.com/marceloboyano/challengeHoroscopeApi.git
cd challengeHoroscopeApi
```

### 2. Configure the connection string

Edit `HoroscopeApi/appsettings.json` and point it to your SQL Server instance:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=HoroscopeDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```

### 3. Create the database

Either option works (see [`database/README.md`](database/README.md) for details):

```bash
# Option A — one command, includes anonymized sample data
sqlcmd -S YOUR_SERVER -E -i database/HoroscopeDb.sql

# Option B — schema only, via EF Core migrations
dotnet ef database update --project HoroscopeApi
```

### 4. Run

```bash
dotnet run --project HoroscopeApi
```

Swagger UI opens at **`https://localhost:7194/swagger`**.

---

## Authentication

1. `POST /api/auth/register` or `POST /api/auth/login` to obtain a JWT.
2. In Swagger click **Authorize** and enter `Bearer {token}`.
3. Call the protected endpoints.

Sample users seeded by the SQL script (password is the same for both):

| Username    | Password    |
| ----------- | ----------- |
| `demo_user` | `Demo1234!` |
| `jane_doe`  | `Demo1234!` |

---

## API endpoints

| Method | Route | Auth | Description |
| --- | --- | --- | --- |
| `POST` | `/api/auth/register` | Public | Register a user, returns a JWT |
| `POST` | `/api/auth/login` | Public | Authenticate, returns a JWT |
| `GET` | `/api/profile` | Bearer | Get the authenticated user's profile |
| `PUT` | `/api/profile` | Bearer | Partially update the profile (no username change) |
| `GET` | `/api/horoscope/today` | Bearer | Daily horoscope + sign + days to next birthday |
| `GET` | `/api/stats/most-searched` | Bearer | Most searched sign across the system |
| `GET` | `/api/stats/ranking` | Bearer | Sign ranking by number of queries |
| `GET` | `/api/stats/history?pageNumber=1&pageSize=10` | Bearer | User's paginated query history |

All responses use a consistent envelope:

```json
{ "success": true, "message": "Success", "data": { }, "statusCode": 200 }
```

### Example

```http
POST /api/auth/login
Content-Type: application/json

{ "username": "demo_user", "password": "Demo1234!" }
```

---

## Architecture

Classic layered design: **Controller → Service → Repository → DbContext**, with
each layer depending on interfaces (testable, decoupled). Full class-by-class

```
HoroscopeApi/
  Controllers/      thin controllers (ApiControllerBase helpers)
  Services/         business logic, returns ServiceResult<T>
  Repositories/     generic base + specific repositories (EF Core)
  DataAccess/       AppDbContext (Fluent API)
  Entities/         User, HoroscopeQuery
  DTOs/             Requests / Responses / External + ApiResponse, PagedResponse
  Validators/       FluentValidation rules
  Mappings/         manual entity -> DTO mapping
  Helpers/          ZodiacCalculator, BirthdayCalculator (pure, testable)
  Config/           JWT/external settings, middleware, validation filter, JSON converters
  Constants/        centralized Messages and RegexPatterns
  Migrations/       EF Core Code First migrations
HoroscopeApi.Tests/ xUnit + Moq
database/           one-command restore script + docs
```

---

## Testing

```bash
dotnet test
```

55 tests covering the zodiac/birthday helpers, the auth and horoscope services
(including the cache-hits-external-API-only-once guarantee), the user service and
the validators.

---

## Design decisions (what & why)

- **Layered + interfaces** — separation of concerns and mockable dependencies.
- **`ServiceResult<T>` + `ApiResponse<T>`** — communicate outcomes/HTTP codes
  without throwing for expected flows, and a single consistent response shape.
- **DTOs separated from entities** — never leak `PasswordHash`, prevent
  over-posting, decouple the API from the DB schema.
- **Manual mapping** instead of AutoMapper — AutoMapper went commercial and v14
  had a known vulnerability; manual mapping is dependency-free and explicit.
- **FluentValidation via an action filter** — expressive, centralized, automatic
  validation; controllers stay clean.
- **No Unit of Work** — EF Core's `DbContext` already is one and every use case is
  a single write; an explicit transaction (owned by the service) would be added
  only if atomic multi-writes were needed.
- **Cache by `sign + date`** — the horoscope is identical for everyone of the same
  sign on a given day, so the external API is called at most once per sign/day.
- **`StringContent` instead of `PostAsJsonAsync`** — the external (Vercel) edge
  rejects chunked transfer-encoding; sending an explicit `Content-Length` fixes a
  real 400 that was debugged.
- **Centralized messages** (`Constants/Messages.cs`) — consistency and a natural
  seam for future i18n.
- **JWT hardening** — cleared inbound claim map (so `sub` is read as-is) and
  `ClockSkew = TimeSpan.Zero` (exact expiration).
- **Custom `DateOnly` JSON converter** — treats an empty/blank `birthDate` as
  "not provided", so partial `PUT` updates are forgiving.

## Improvements I'd make with more time

- Refresh tokens / token revocation (a `jti` claim is already issued).
- Integration tests with `WebApplicationFactory`.
- A distributed cache (e.g. Redis) so caching works across multiple instances.
- Rate limiting on the auth endpoints.
- Health checks and structured logging/correlation IDs.
- Full i18n via `.resx` / `IStringLocalizer`.

## Known issues / not addressed

- The JWT signing key lives in `appsettings.json` for convenience; in a real
  deployment it should come from user secrets / environment variables / a vault.
- The in-memory cache is per-instance (fine for this challenge, not for a scaled
  out deployment — see Redis note above).
