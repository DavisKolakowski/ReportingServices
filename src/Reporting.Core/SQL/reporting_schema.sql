-- Create Schema
BEGIN TRY
    BEGIN TRANSACTION;

    IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'Reporting')
    BEGIN
        EXEC ('CREATE SCHEMA [Reporting] AUTHORIZATION [dbo]');
    END;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO