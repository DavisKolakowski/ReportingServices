namespace Reporting.Core.Services
{
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using Reporting.Core.Contracts;
    using System.Data;

    public class DapperConnectionService : IDapperConnectionService
    {
        private readonly string _connectionString;

        public DapperConnectionService(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("ReportingDatabase");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new System.Exception("Connection string is missing from the configuration");
            }
            this._connectionString = connectionString;
        }

        public IDbConnection GetConnection()
        {
            return new SqlConnection(this._connectionString);
        }
    }
}
