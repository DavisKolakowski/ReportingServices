USE [AQT_DataMart]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [Reporting].[DataSourceUsageForDateRange]
    @FirstCountDate DATE,
    @LastCountDate DATE
AS
BEGIN
    SET NOCOUNT ON;  

    IF @FirstCountDate IS NULL OR @LastCountDate IS NULL OR @FirstCountDate > @LastCountDate
    BEGIN
        RAISERROR('Invalid date range provided.', 16, 1);
        RETURN;
    END;

    WITH [Segments] AS (
        SELECT 
            JSON_VALUE([F].value, '$.dataDictionaryId') AS [DataDictionaryId],
            [S].[CampaignSegmentId],
            [S].[CampaignGroupId],
            'Campaign Segment' AS [Type]
        FROM [dbo].[CampaignGroup] [C]
            INNER JOIN [dbo].[CampaignSegment] [S] ON [C].[CampaignGroupId] = [S].[CampaignGroupId]
            INNER JOIN [dbo].[Query] [Q] ON [S].[QueryId] = [Q].[QueryId]
            CROSS APPLY OPENJSON([Q].[SerializedObject], '$.fieldGroups') AS [FG]
            CROSS APPLY OPENJSON([FG].value, '$.fields') AS [F]
        WHERE [C].[FlightStartDate] BETWEEN @FirstCountDate AND @LastCountDate
            AND [C].[StatusCode] IN ('Active', 'Expired', 'Closed')
            AND JSON_VALUE([F].value, '$.dataDictionaryId') <> 0
        GROUP BY 
            JSON_VALUE([F].value, '$.dataDictionaryId'),
            [S].[CampaignSegmentId],
            [S].[CampaignGroupId]
    ), [Adobe] AS (
        SELECT 
            [DD2].[DataDictionaryId],
            NULL AS [CampaignSegmentId],
            NULL AS [CampaignGroupId],
            '[Adobe] Trait' AS [Type]
        FROM [dbo].[AdobeTrait] [AT]
            INNER JOIN [dbo].[DataDictionary] [DD2] ON [DD2].[DataDictionaryId] = [AT].[DataDictionaryId]
        WHERE [DD2].[ActiveFlag] = 1
    ), [Combined] AS (
        SELECT * FROM [Segments]
        UNION ALL 
        SELECT * FROM [Adobe]
    ), [RawData] AS (
        SELECT 
            [DD].[DataDictionaryId],
            [DD].[SourceName],
            [DD].[ColumnName],
            COUNT([S].[CampaignSegmentId]) AS [SegmentCount]
        FROM [Combined] [S]
            INNER JOIN [dbo].[DataDictionary] [DD] ON [S].[DataDictionaryId] = [DD].[DataDictionaryId]
        WHERE [DD].[ActiveFlag] = 1
        GROUP BY 
            [DD].[ColumnName],
            [DD].[SourceName],
            [DD].[DataDictionaryId],
            [DD].[ColumnName]
    ), [Results] AS (
        SELECT 
            [DD].[DataDictionaryId] AS [DataDictionaryId],
            ISNULL([R].[SourceName], [DD].[SourceName]) AS [SourceName],
            ISNULL([R].[ColumnName], [DD].[ColumnName]) AS [ColumnName],
            ISNULL([R].[SegmentCount], 0) AS [SegmentCount],
            CASE 
                WHEN [A].[DataDictionaryId] IS NULL THEN 'No' 
                ELSE 'Yes' 
            END AS [IncludedInAdobeOverwrite],
            CONVERT(VARCHAR(10), CAST([DD].[DateCreated] AS DATE), 101) AS [DateCreated]
        FROM [RawData] [R]
            LEFT JOIN [Adobe] [A] ON [A].[DataDictionaryId] = [R].[DataDictionaryId]
            RIGHT JOIN [dbo].[DataDictionary] [DD] ON [DD].[DataDictionaryId] = [R].[DataDictionaryId]
        WHERE [DD].[ActiveFlag] = 1
    )
    SELECT
        ROW_NUMBER() OVER (
            ORDER BY 
                [SegmentCount] DESC, 
                [IncludedInAdobeOverwrite] DESC, 
                [SourceName], 
                [ColumnName]
        ) AS [InternalId],    
        [DataDictionaryId],
        [SourceName],
        [ColumnName],
        [SegmentCount],
        [DateCreated]
    FROM [Results];
END
GO
