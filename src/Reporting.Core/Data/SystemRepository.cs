namespace Reporting.Core.Data
{
    using Dapper;

    using Reporting.Core.Contracts;
    using Reporting.Core.Entities;

    public class SystemRepository : ISystemRepository
    {
        private readonly IDapperConnectionService _connectionService;

        public SystemRepository(IDapperConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        public async Task<IEnumerable<ReportParameter>> GetReportParametersAsync(string sqlObjectName)
        {
            using (var connection = _connectionService.GetConnection())
            {
                var sql = @"
                DECLARE @ObjectName NVARCHAR(128) = PARSENAME(@SqlObjectName, 1);
                DECLARE @SchemaName NVARCHAR(128) = PARSENAME(@SqlObjectName, 2);

                SELECT 
                    p.parameter_id AS [Position],
                    p.name AS [Name],
                    CASE 
                        WHEN p.has_default_value = 1 THEN 
                            CONVERT(sql_variant, p.default_value)
                        ELSE NULL
                    END AS [CurrentValue],
                    t.name AS [SqlDataType],
                    p.has_default_value AS [HasDefaultValue]
                FROM 
                    sys.parameters p
                JOIN 
                    sys.types t ON p.user_type_id = t.user_type_id
                JOIN
                    sys.objects o ON p.object_id = o.object_id
                WHERE 
                    o.name = @ObjectName 
                    AND o.schema_id = SCHEMA_ID(@SchemaName) 
                    AND o.type IN ('P', 'PC') -- P for procedures, PC for CLR procedures
                    AND t.is_user_defined = 0 -- Ensure we're only getting system types
                ORDER BY 
                    p.parameter_id;
                ";

                var parameters = await connection.QueryAsync<ReportParameter>(sql, new { SqlObjectName = sqlObjectName });
                return parameters;
            }
        }

        public async Task<IEnumerable<ReportColumnDefinition>> GetReportColumnDefinitionsAsync(string sqlObjectName)
        {
            using (var connection = _connectionService.GetConnection())
            {
                var sql = @"
                DECLARE @ObjectId INT = OBJECT_ID(@SqlObjectName);

                IF OBJECTPROPERTY(@ObjectId, 'IsProcedure') = 1
                BEGIN
                    SELECT 
                        column_ordinal AS [ColumnId],
                        name AS [Name],
                        system_type_name AS [SqlDataType],
                        is_nullable AS [IsNullable],
                        CASE 
                            WHEN name = 'InternalId' THEN 1 
                            ELSE 0 
                        END AS [IsIdentity]
                    FROM 
                        sys.dm_exec_describe_first_result_set_for_object(@ObjectId, NULL);
                END
                ELSE
                BEGIN
                    SELECT 
                        c.column_id AS [ColumnId],
                        c.name AS [Name],
                        t.name AS [SqlDataType],
                        c.is_nullable AS [IsNullable],
                        CASE 
                            WHEN c.name = 'InternalId' THEN 1 
                            ELSE c.is_identity 
                        END AS [IsIdentity]
                    FROM 
                        sys.columns c
                    JOIN 
                        sys.types t ON c.user_type_id = t.user_type_id
                    JOIN
                        sys.objects o ON c.object_id = o.object_id
                    WHERE 
                        o.object_id = @ObjectId AND o.type IN ('U', 'V') -- U for tables, V for views
                    ORDER BY 
                        c.column_id;
                END;
                ";

                var columns = await connection.QueryAsync<ReportColumnDefinition>(sql, new { SqlObjectName = sqlObjectName });
                return columns;
            }
        }
    }
}
