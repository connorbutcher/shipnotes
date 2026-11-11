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

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261110203454_AddMediaData'
)
BEGIN
    CREATE TABLE [MediaData] (
        [Id] uniqueidentifier NOT NULL,
        [ApplicationId] uniqueidentifier NOT NULL,
        [ReleaseNoteId] uniqueidentifier NULL,
        [FileName] nvarchar(255) NOT NULL,
        [ContentType] varchar(100) NOT NULL,
        [SizeBytes] bigint NOT NULL,
        [StoragePath] nvarchar(400) NOT NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NOT NULL,
        CONSTRAINT [PK_MediaData] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_MediaData_Applications_ApplicationId] FOREIGN KEY ([ApplicationId]) REFERENCES [Applications] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_MediaData_ReleaseNotes_ReleaseNoteId] FOREIGN KEY ([ReleaseNoteId]) REFERENCES [ReleaseNotes] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261110203454_AddMediaData'
)
BEGIN
    CREATE INDEX [IX_MediaData_ApplicationId] ON [MediaData] ([ApplicationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261110203454_AddMediaData'
)
BEGIN
    CREATE INDEX [IX_MediaData_ReleaseNoteId] ON [MediaData] ([ReleaseNoteId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261110203454_AddMediaData'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261110203454_AddMediaData', N'10.0.14');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261110214513_AddReleaseNoteSections'
)
BEGIN
    EXEC sp_rename N'[ReleaseNotes].[ContentHtml]', N'SummaryHtml', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261110214513_AddReleaseNoteSections'
)
BEGIN
    CREATE TABLE [ReleaseNoteSections] (
        [Category] varchar(20) NOT NULL,
        [ReleaseNoteId] uniqueidentifier NOT NULL,
        [ContentHtml] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_ReleaseNoteSections] PRIMARY KEY ([ReleaseNoteId], [Category]),
        CONSTRAINT [FK_ReleaseNoteSections_ReleaseNotes_ReleaseNoteId] FOREIGN KEY ([ReleaseNoteId]) REFERENCES [ReleaseNotes] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261110214513_AddReleaseNoteSections'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261110214513_AddReleaseNoteSections', N'10.0.14');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261111200953_StoreReleaseNotesAsOneDocument'
)
BEGIN
    EXEC sp_rename N'[ReleaseNotes].[SummaryHtml]', N'ContentHtml', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261111200953_StoreReleaseNotesAsOneDocument'
)
BEGIN
    UPDATE n
    SET ContentHtml = n.ContentHtml + s.Html
    FROM ReleaseNotes AS n
    CROSS APPLY (
        SELECT STRING_AGG(
            CAST(N'<section class="note-section note-section--' + LOWER(x.Category) + N'">'
                + N'<h2 class="note-section__title">'
                + CASE x.Category WHEN 'Breaking' THEN N'Breaking changes' ELSE CAST(x.Category AS nvarchar(20)) END
                + N'</h2>' + x.ContentHtml + N'</section>' AS nvarchar(max)),
            N'') WITHIN GROUP (ORDER BY CASE x.Category
                WHEN 'Breaking' THEN 0 WHEN 'Added' THEN 1 WHEN 'Changed' THEN 2 WHEN 'Deprecated' THEN 3
                WHEN 'Removed' THEN 4 WHEN 'Fixed' THEN 5 WHEN 'Security' THEN 6 ELSE 7 END) AS Html
        FROM ReleaseNoteSections AS x
        WHERE x.ReleaseNoteId = n.Id
    ) AS s
    WHERE s.Html IS NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261111200953_StoreReleaseNotesAsOneDocument'
)
BEGIN
    DROP TABLE [ReleaseNoteSections];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261111200953_StoreReleaseNotesAsOneDocument'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261111200953_StoreReleaseNotesAsOneDocument', N'10.0.14');
END;

COMMIT;
GO

