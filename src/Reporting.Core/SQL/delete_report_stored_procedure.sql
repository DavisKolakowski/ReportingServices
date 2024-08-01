CREATE OR ALTER PROCEDURE [Reporting].[DeleteReport]
    @Key NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM [Reporting].[Reports]
    WHERE [Key] = @Key;
END;
GO
