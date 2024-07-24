namespace Reporting.Core.Services
{
    using Microsoft.Data.SqlClient;

    using Reporting.Core.Contracts;

    using System.Data;

    public class DapperConnectionService : IDapperConnectionService
    {
        private readonly string _connectionString;

        public DapperConnectionService(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new System.Exception("Connection string is missing");
            }
            this._connectionString = connectionString;
        }

        public IDbConnection GetConnection()
        {
            return new SqlConnection(this._connectionString);
        }
    }
}
