# Database

The API uses **SQL Server** with **EF Core (Code First)**. The schema lives in the
EF migrations (`HoroscopeApi/Migrations`). This folder provides a ready-to-run
script to recreate the database with sample data.

## Connection string

Set your SQL Server instance in `HoroscopeApi/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=HoroscopeDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```

Replace `YOUR_SERVER` with your instance name (e.g. `localhost`, `.\SQLEXPRESS`,
`(localdb)\MSSQLLocalDB`, etc.).

## Option A - One command (schema + sample data)

Recreates the database, applies the schema and seeds anonymized sample data.
Idempotent: safe to run more than once.

```bash
sqlcmd -S YOUR_SERVER -E -i database/HoroscopeDb.sql
```

## Option B - EF Core migrations (schema only)

Requires the EF tools (`dotnet tool install --global dotnet-ef`).

```bash
dotnet ef database update --project HoroscopeApi
```

## Sample users

Both demo users share the same password:

| Username    | Email                  | Password    |
| ----------- | ---------------------- | ----------- |
| `demo_user` | `demo@example.com`     | `Demo1234!` |
| `jane_doe`  | `jane.doe@example.com` | `Demo1234!` |

Use them on `POST /api/auth/login` to get a JWT and try the protected endpoints.

> Note: `Id` columns are auto-incremental (`IDENTITY(1,1)`).
