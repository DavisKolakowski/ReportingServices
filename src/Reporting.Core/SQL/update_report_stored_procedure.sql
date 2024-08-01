CREATE OR ALTER PROCEDURE [Reporting].[UpdateReport]
    @Key NVARCHAR(50),
    @Name NVARCHAR(100) = NULL,
    @Description NVARCHAR(500) = NULL,
    @IsActive BIT = NULL,
    @UpdatedByUser NVARCHAR(128) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [Reporting].[Reports]
    SET
        [Name] = ISNULL(@Name, [Name]),
        [Description] = ISNULL(@Description, [Description]),
        [UpdatedByUser] = ISNULL(@UpdatedByUser, SUSER_NAME()),
        [UpdatedAtDate] = GETDATE(),
        [IsActive] = ISNULL(@IsActive, [IsActive])
    WHERE [Key] = @Key;
END;
GO
