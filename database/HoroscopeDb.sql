/* =============================================================================
   HoroscopeDb - full restore script (schema + sample data)
   -----------------------------------------------------------------------------
   Creates the database (if missing), applies the EF Core schema (idempotent),
   and seeds anonymized sample data.

   Run it with one command (adjust the server name to your SQL Server instance):

       sqlcmd -S <server> -E -i database\HoroscopeDb.sql

   Example (named instance "MSB", Windows auth):

       sqlcmd -S MSB -E -i database\HoroscopeDb.sql

   The script is idempotent: running it again will not duplicate rows.

   Sample login (both demo users share the same password):
       username: demo_user   password: Demo1234!
       username: jane_doe    password: Demo1234!
   ============================================================================= */

IF DB_ID(N'HoroscopeDb') IS NULL
BEGIN
    CREATE DATABASE [HoroscopeDb];
END;
GO

USE [HoroscopeDb];
GO

/* ---------------------------------------------------------------------------
   Schema (generated from EF Core migrations - idempotent)
   --------------------------------------------------------------------------- */
IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260622224622_InitialCreate'
)
BEGIN
    CREATE TABLE [Users] (
        [Id] int NOT NULL IDENTITY,
        [Username] nvarchar(50) NOT NULL,
        [Email] nvarchar(150) NOT NULL,
        [PasswordHash] nvarchar(max) NOT NULL,
        [BirthDate] date NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260622224622_InitialCreate'
)
BEGIN
    CREATE TABLE [HoroscopeQueries] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [Sign] nvarchar(20) NOT NULL,
        [QueryDate] date NOT NULL,
        [ResultText] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_HoroscopeQueries] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HoroscopeQueries_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260622224622_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_HoroscopeQueries_Sign] ON [HoroscopeQueries] ([Sign]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260622224622_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_HoroscopeQueries_UserId] ON [HoroscopeQueries] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260622224622_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Users_Username] ON [Users] ([Username]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260622224622_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260622224622_InitialCreate', N'10.0.9');
END;

COMMIT;
GO

/* ---------------------------------------------------------------------------
   Sample data (anonymized). Password for both users: Demo1234!
   --------------------------------------------------------------------------- */
SET IDENTITY_INSERT [Users] ON;

IF NOT EXISTS (SELECT 1 FROM [Users] WHERE [Id] = 1)
    INSERT INTO [Users] ([Id], [Username], [Email], [PasswordHash], [BirthDate], [CreatedAt])
    VALUES (1, N'demo_user', N'demo@example.com', N'$2a$11$zj6W06M87j7Atf0wz.ZBKuCopYCINo8uM4BLErdMZhzl0d1tc0QZG', '1990-05-20', '2026-06-20T10:00:00');

IF NOT EXISTS (SELECT 1 FROM [Users] WHERE [Id] = 2)
    INSERT INTO [Users] ([Id], [Username], [Email], [PasswordHash], [BirthDate], [CreatedAt])
    VALUES (2, N'jane_doe', N'jane.doe@example.com', N'$2a$11$2IZ2qIF7yW/ZHQrUuJdzN.Jn6QelOM9Kv8iiZ9ssNfFsSYq9mnrmG', '1988-11-02', '2026-06-20T10:05:00');

SET IDENTITY_INSERT [Users] OFF;
GO

SET IDENTITY_INSERT [HoroscopeQueries] ON;

IF NOT EXISTS (SELECT 1 FROM [HoroscopeQueries] WHERE [Id] = 1)
    INSERT INTO [HoroscopeQueries] ([Id], [UserId], [Sign], [QueryDate], [ResultText], [CreatedAt])
    VALUES (1, 1, N'Taurus', '2026-06-20', N'Today brings a calm and steady energy. A good moment to focus on what truly matters and to take care of small pending tasks.', '2026-06-20T10:10:00');

IF NOT EXISTS (SELECT 1 FROM [HoroscopeQueries] WHERE [Id] = 2)
    INSERT INTO [HoroscopeQueries] ([Id], [UserId], [Sign], [QueryDate], [ResultText], [CreatedAt])
    VALUES (2, 2, N'Scorpio', '2026-06-20', N'Your intuition is sharp today. Trust your instincts when making decisions and avoid unnecessary conflicts.', '2026-06-20T10:15:00');

IF NOT EXISTS (SELECT 1 FROM [HoroscopeQueries] WHERE [Id] = 3)
    INSERT INTO [HoroscopeQueries] ([Id], [UserId], [Sign], [QueryDate], [ResultText], [CreatedAt])
    VALUES (3, 1, N'Taurus', '2026-06-21', N'A great day for connecting with friends. Communication flows easily and new opportunities may appear.', '2026-06-21T09:30:00');

SET IDENTITY_INSERT [HoroscopeQueries] OFF;
GO
