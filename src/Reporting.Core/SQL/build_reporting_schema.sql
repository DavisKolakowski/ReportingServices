IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'Reporting')
BEGIN
    EXEC ('CREATE SCHEMA [Reporting] AUTHORIZATION [dbo]');
END;
