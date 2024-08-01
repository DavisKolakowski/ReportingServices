namespace Reporting.Core.Data
{
    using System.Data;
    using System.Dynamic;

    using Dapper;

    using DocumentFormat.OpenXml;
    using DocumentFormat.OpenXml.Packaging;
    using DocumentFormat.OpenXml.Spreadsheet;

    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;

    using Reporting.Core.Contracts;
    using Reporting.Core.Entities;
    using Reporting.Core.Enums;
    using Reporting.Core.Helpers;
    using Reporting.Core.Models;
    using Reporting.Core.Utilities;

    public class ReportRepository : IReportRepository
    {
        private readonly IDapperConnectionService _connectionService;
        private readonly ILogger<ReportRepository> _logger;

        public ReportRepository(IDapperConnectionService connectionService, ILogger<ReportRepository> logger)
        {
            _connectionService = connectionService;
            _logger = logger;
        }

        public async Task<IEnumerable<Report>> GetAllAsync()
        {
            using (var connection = _connectionService.GetConnection())
            {
                var sql = @"
                SELECT 
                    [R].[Id], 
                    [R].[Key], 
                    [R].[Name], 
                    [R].[Description], 
                    [R].[CreatedByUser], 
                    [R].[CreatedAtDate], 
                    [R].[UpdatedByUser], 
                    [R].[UpdatedAtDate], 
                    [R].[IsActive],
                    [R].[ReportSourceId],
                    (CASE WHEN EXISTS (
                        SELECT 1
                        FROM sys.parameters p
                        JOIN sys.objects o ON p.object_id = o.object_id
                        WHERE o.name = [RS].[Name] AND o.schema_id = SCHEMA_ID([RS].[Schema]) AND o.type IN ('P', 'PC')
                    ) THEN 1 ELSE 0 END) AS [HasParameters]
                FROM 
                    [Reporting].[Reports] [R]
                JOIN
                    [Reporting].[ReportSources] [RS] ON [R].[ReportSourceId] = [RS].[Id];
                ";

                var reports = await connection.QueryAsync<Report>(sql);
                return reports;
            }
        }

        public async Task<IEnumerable<Report>> GetAllActiveAsync()
        {
            using (var connection = _connectionService.GetConnection())
            {
                var sql = @"
                SELECT 
                    [R].[Id], 
                    [R].[Key], 
                    [R].[Name], 
                    [R].[Description], 
                    [R].[CreatedByUser], 
                    [R].[CreatedAtDate], 
                    [R].[UpdatedByUser], 
                    [R].[UpdatedAtDate], 
                    [R].[IsActive],
                    [R].[ReportSourceId],
                    (CASE WHEN EXISTS (
                        SELECT 1
                        FROM sys.parameters p
                        JOIN sys.objects o ON p.object_id = o.object_id
                        WHERE o.name = [RS].[Name] AND o.schema_id = SCHEMA_ID([RS].[Schema]) AND o.type IN ('P', 'PC')
                    ) THEN 1 ELSE 0 END) AS [HasParameters]
                FROM 
                    [Reporting].[Reports] [R]
                JOIN
                    [Reporting].[ReportSources] [RS] ON [R].[ReportSourceId] = [RS].[Id]
                WHERE 
                    [R].[IsActive] = 1;
                ";

                var reports = await connection.QueryAsync<Report>(sql);
                return reports;
            }
        }

        public async Task<Report?> GetByKeyAsync(string reportKey)
        {
            using (var connection = _connectionService.GetConnection())
            {
                var sql = @"
                SELECT 
                    [R].[Id], 
                    [R].[Key], 
                    [R].[Name], 
                    [R].[Description], 
                    [R].[CreatedByUser], 
                    [R].[CreatedAtDate], 
                    [R].[UpdatedByUser], 
                    [R].[UpdatedAtDate], 
                    [R].[IsActive],
                    [R].[ReportSourceId],
                    (CASE WHEN EXISTS (
                        SELECT 1
                        FROM sys.parameters p
                        JOIN sys.objects o ON p.object_id = o.object_id
                        WHERE o.name = [RS].[Name] AND o.schema_id = SCHEMA_ID([RS].[Schema]) AND o.type IN ('P', 'PC')
                    ) THEN 1 ELSE 0 END) AS [HasParameters]
                FROM 
                    [Reporting].[Reports] [R]
                JOIN
                    [Reporting].[ReportSources] [RS] ON [R].[ReportSourceId] = [RS].[Id]
                WHERE 
                    [R].[Key] = @ReportKey;
                ";

                var report = await connection.QuerySingleOrDefaultAsync<Report>(sql, new { ReportKey = reportKey });
                return report;
            }
        }

        public async Task<Report?> GetByIdAsync(int reportId)
        {
            using (var connection = _connectionService.GetConnection())
            {
                var sql = @"
                SELECT 
                    [R].[Id], 
                    [R].[Key], 
                    [R].[Name], 
                    [R].[Description], 
                    [R].[CreatedByUser], 
                    [R].[CreatedAtDate], 
                    [R].[UpdatedByUser], 
                    [R].[UpdatedAtDate], 
                    [R].[IsActive],
                    [R].[ReportSourceId],
                    (CASE WHEN EXISTS (
                        SELECT 1
                        FROM sys.parameters p
                        JOIN sys.objects o ON p.object_id = o.object_id
                        WHERE o.name = [RS].[Name] AND o.schema_id = SCHEMA_ID([RS].[Schema]) AND o.type IN ('P', 'PC')
                    ) THEN 1 ELSE 0 END) AS [HasParameters]
                FROM 
                    [Reporting].[Reports] [R]
                JOIN
                    [Reporting].[ReportSources] [RS] ON [R].[ReportSourceId] = [RS].[Id]
                WHERE 
                    [R].[Id] = @ReportId;
                ";

                var report = await connection.QuerySingleOrDefaultAsync<Report>(sql, new { ReportId = reportId });
                return report;
            }
        }

        public async Task<DataTable> ExecuteAsync(ReportSource source, ReportColumnDefinition[] columns, ReportParameter[]? parameters = null)
        {
            using (var connection = _connectionService.GetConnection())
            {
                try
                {
                    var dataTable = new DataTable();

                    foreach (var column in columns)
                    {
                        dataTable.Columns.Add(column.Name, ObjectHelpers.GetCSharpTypeForSqlTypeString(column.SqlDataType!));
                    }

                    switch (source.Type)
                    {
                        case ReportSourceType.Procedure:
                            var dynamicParameters = CreateDynamicParameters(parameters);
                            await ExecuteProcedureAsync(connection, source.FullName, dynamicParameters, dataTable);
                            break;
                        case ReportSourceType.View:
                        default:
                            await ExecuteViewAsync(connection, source.FullName, dataTable);
                            break;
                    }

                    return dataTable;
                }
                catch (SqlException ex)
                {
                    _logger.LogError(ex, "An error occurred while executing the report: {Message}", ex.Message);
                    throw;
                }
            }
        }

        private async Task ExecuteProcedureAsync(IDbConnection connection, string procedureName, DynamicParameters parameters, DataTable dataTable)
        {
            using (var reader = await connection.ExecuteReaderAsync(procedureName, parameters, commandType: CommandType.StoredProcedure))
            {
                dataTable.Load(reader);
            }
        }

        private async Task ExecuteViewAsync(IDbConnection connection, string viewName, DataTable dataTable)
        {
            var sql = $"SELECT * FROM {viewName}";
            using (var reader = await connection.ExecuteReaderAsync(sql))
            {
                dataTable.Load(reader);
            }
        }

        private DynamicParameters CreateDynamicParameters(ReportParameter[]? parameters)
        {
            var dynamicParameters = new DynamicParameters();
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    var currentValueString = param.CurrentValue?.ToString();
                    if (string.IsNullOrEmpty(param.Name))
                    {
                        continue;
                    }
                    _logger.LogInformation("Parameter {Name} = {Value}", param.Name, currentValueString);
                    dynamicParameters.Add(param.Name, ObjectHelpers.ConvertSqlValue(param.CurrentValue, param.SqlDataType));
                }
            }

            return dynamicParameters;
        }

        public async Task<Report> CreateAsync(Report report)
        {
            using (var connection = _connectionService.GetConnection())
            {
                var sql = @"
                INSERT INTO [Reporting].[Reports] (
                    [Key], 
                    [Name], 
                    [Description], 
                    [CreatedByUser], 
                    [CreatedAtDate], 
                    [UpdatedByUser], 
                    [UpdatedAtDate], 
                    [IsActive], 
                    [ReportSourceId]
                ) VALUES (
                    @Key, 
                    @Name, 
                    @Description, 
                    @CreatedByUser, 
                    @CreatedAtDate, 
                    @UpdatedByUser, 
                    @UpdatedAtDate, 
                    @IsActive, 
                    @ReportSourceId
                );
                SELECT CAST(SCOPE_IDENTITY() as int);
                ";

                var reportId = await connection.QuerySingleAsync<int>(sql, report);
                report.Id = reportId;
                return report;
            }
        }

        public async Task<Report> UpdateAsync(Report report)
        {
            using (var connection = _connectionService.GetConnection())
            {
                var sql = @"
                UPDATE [Reporting].[Reports]
                SET 
                    [Key] = @Key, 
                    [Name] = @Name, 
                    [Description] = @Description, 
                    [UpdatedByUser] = @UpdatedByUser, 
                    [UpdatedAtDate] = @UpdatedAtDate, 
                    [IsActive] = @IsActive
                WHERE 
                    [Id] = @Id;
                ";

                await connection.ExecuteAsync(sql, report);
                return report;
            }
        }

        public async Task DeleteAsync(Report report)
        {
            using (var connection = _connectionService.GetConnection())
            {
                var sql = @"
                DELETE FROM [Reporting].[Reports]
                WHERE [Id] = @Id;
                ";

                await connection.ExecuteAsync(sql, new { report.Id });
            }
        }
    }
}
