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
    WHERE [MigrationId] = N'20261110195120_InitialCreate'
)
BEGIN
    CREATE TABLE [Applications] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [IconUrl] nvarchar(2048) NULL,
        [ApiKeyHash] char(64) NOT NULL,
        [ApiKeyPrefix] varchar(12) NOT NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NOT NULL,
        CONSTRAINT [PK_Applications] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261110195120_InitialCreate'
)
BEGIN
    CREATE TABLE [ReleaseNotes] (
        [Id] uniqueidentifier NOT NULL,
        [ApplicationId] uniqueidentifier NOT NULL,
        [Version] varchar(128) NOT NULL,
        [VersionSortKey] varchar(400) COLLATE Latin1_General_100_BIN2 NOT NULL,
        [IsPreRelease] bit NOT NULL,
        [Title] nvarchar(200) NOT NULL,
        [ContentHtml] nvarchar(max) NOT NULL,
        [PublishedAt] datetimeoffset NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NOT NULL,
        CONSTRAINT [PK_ReleaseNotes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ReleaseNotes_Applications_ApplicationId] FOREIGN KEY ([ApplicationId]) REFERENCES [Applications] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261110195120_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Applications_ApiKeyHash] ON [Applications] ([ApiKeyHash]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261110195120_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Applications_Name] ON [Applications] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261110195120_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ReleaseNotes_ApplicationId_PublishedAt_VersionSortKey] ON [ReleaseNotes] ([ApplicationId], [PublishedAt], [VersionSortKey]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261110195120_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ReleaseNotes_ApplicationId_VersionSortKey] ON [ReleaseNotes] ([ApplicationId], [VersionSortKey]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261110195120_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261110195120_InitialCreate', N'10.0.14');
END;

COMMIT;
GO

