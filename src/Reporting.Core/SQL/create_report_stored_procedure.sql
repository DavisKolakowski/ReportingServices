CREATE OR ALTER PROCEDURE [Reporting].[CreateReport]
    @ReportSourceId INT,
    @Key NVARCHAR(50),
    @Name NVARCHAR(100),
    @Description NVARCHAR(500),
    @IsActive BIT = 1,
    @CreatedByUser NVARCHAR(128) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Ensure the Key only contains lowercase letters, numbers, and underscores
    IF @Key LIKE '%[^a-z0-9_]%' OR LEN(@Key) = 0
    BEGIN
        THROW 50000, 'Key must be lowercase letters, numbers, and underscores only.', 1;
    END

    INSERT INTO [Reporting].[Reports] (
        [ReportSourceId], [Key], [Name], [Description], [CreatedByUser], [CreatedAtDate], [IsActive]
    )
    VALUES (
        @ReportSourceId, @Key, @Name, @Description, ISNULL(@CreatedByUser, SUSER_NAME()), GETDATE(), @IsActive
    );
END;
GO
