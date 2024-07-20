namespace Reporting.Core.Data
{
    using Dapper;

    using Microsoft.Extensions.Logging;
    using Reporting.Core.Contracts;
    using Reporting.Core.Entities;

    public class ReportSourceRepository : IReportSourceRepository
    {
        private readonly IDapperConnectionService _connectionService;
        private readonly ILogger<ReportSourceRepository> _logger;

        public ReportSourceRepository(IDapperConnectionService connectionService, ILogger<ReportSourceRepository> logger)
        {
            _connectionService = connectionService;
            _logger = logger;
        }

        public async Task<ReportSource?> GetByIdAsync(int reportSourceId)
        {
            using (var connection = _connectionService.GetConnection())
            {
                var sql = @"
                SELECT 
                    [S].[Id], 
                    [S].[Type], 
                    [S].[Schema], 
                    [S].[Name], 
                    [S].[LastActivityType], 
                    [S].[LastActivityByUser], 
                    [S].[LastActivityDate]
                FROM 
                    [Reporting].[ReportSources] [S]
                WHERE 
                    [S].[Id] = @ReportSourceId;
                ";

                var reportSource = await connection.QuerySingleOrDefaultAsync<ReportSource>(sql, new { ReportSourceId = reportSourceId });
                return reportSource;
            }
        }

        public async Task<IEnumerable<ReportSource>> GetUnusedAsync()
        {
            using (var connection = _connectionService.GetConnection())
            {
                var sql = @"
                SELECT 
                    [S].[Id], 
                    [S].[Type], 
                    [S].[Schema], 
                    [S].[Name], 
                    [S].[LastActivityType], 
                    [S].[LastActivityByUser], 
                    [S].[LastActivityDate]
                FROM 
                    [Reporting].[ReportSources] [S]
                WHERE 
                    NOT EXISTS (
                        SELECT 1 
                        FROM [Reporting].[Reports] [R]
                        WHERE [R].[ReportSourceId] = [S].[Id]
                    );
                ";

                var sources = await connection.QueryAsync<ReportSource>(sql);
                return sources;
            }
        }

        public async Task<IEnumerable<ReportSourceHistory>> GetActivityHistoryAsync(string sqlObjectName)
        {
            using (var connection = _connectionService.GetConnection())
            {
                var sql = @"
                SELECT 
                    [H].[Id], 
                    [H].[Type], 
                    [H].[SqlObjectName], 
                    [H].[ActivityType], 
                    [H].[ActivityByUser], 
                    [H].[ActivityDate]
                FROM 
                    [Reporting].[ReportSourceHistory] [H]
                WHERE 
                    [H].[SqlObjectName] = @SqlObjectName
                ORDER BY 
                    [H].[ActivityDate];
                ";

                var activities = await connection.QueryAsync<ReportSourceHistory>(sql, new { SqlObjectName = sqlObjectName });
                return activities;
            }
        }
    }
}
