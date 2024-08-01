-- Create Tables
BEGIN TRY
    BEGIN TRANSACTION;

    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ReportSources' AND schema_id = SCHEMA_ID('Reporting'))
    BEGIN
        CREATE TABLE [Reporting].[ReportSources] (
            [Id] INT IDENTITY(1,1) PRIMARY KEY,
            [Type] NVARCHAR(50) NOT NULL,
            [Schema] NVARCHAR(128) NOT NULL,
            [Name] NVARCHAR(128) NOT NULL,
            [LastActivityType] NVARCHAR(50) NOT NULL,
            [LastActivityByUser] NVARCHAR(128) NOT NULL,
            [LastActivityDate] DATETIME2 NOT NULL,
            UNIQUE ([Schema], [Name])
        );
    END;

    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ReportSourceHistory' AND schema_id = SCHEMA_ID('Reporting'))
    BEGIN
        CREATE TABLE [Reporting].[ReportSourceHistory] (
            [Id] INT IDENTITY(1,1) PRIMARY KEY,
            [Type] NVARCHAR(50) NOT NULL,
            [SqlObjectName] NVARCHAR(256) NOT NULL,
            [ActivityType] NVARCHAR(50) NOT NULL,
            [ActivityByUser] NVARCHAR(128) NOT NULL,
            [ActivityDate] DATETIME2 NOT NULL
        );
    END;

    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Reports' AND schema_id = SCHEMA_ID('Reporting'))
    BEGIN
        CREATE TABLE [Reporting].[Reports] (
            [Id] INT IDENTITY(1,1) PRIMARY KEY,
            [Key] NVARCHAR(50) NOT NULL UNIQUE,
            [ReportSourceId] INT NOT NULL FOREIGN KEY REFERENCES [Reporting].[ReportSources]([Id]) ON DELETE CASCADE,
            [Name] NVARCHAR(100) NOT NULL,
            [Description] NVARCHAR(500) NOT NULL,
            [CreatedByUser] NVARCHAR(128) NOT NULL,
            [CreatedAtDate] DATETIME2 NOT NULL,
            [UpdatedByUser] NVARCHAR(128),
            [UpdatedAtDate] DATETIME2,
            [IsActive] BIT NOT NULL DEFAULT 1
        );
    END;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
