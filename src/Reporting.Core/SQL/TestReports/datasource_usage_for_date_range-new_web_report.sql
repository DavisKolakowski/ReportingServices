INSERT INTO [Reporting].[Reports] 
(
    [Key], 
    [ReportSourceId], 
    [Name], 
    [Description], 
    [CreatedByUser], 
    [CreatedAtDate]
)
VALUES 
(
    'datasource_usage_for_date_range',
    (SELECT [Id] FROM [Reporting].[ReportSources] WHERE [Schema] = 'Reporting' AND [Name] = 'DataSourceUsageForDateRange'),
    'Data Source Usage For Date Range', 
    'Report to get data source usage within a specified date range.', 
    SUSER_NAME(), 
    GETDATE()
);
GO