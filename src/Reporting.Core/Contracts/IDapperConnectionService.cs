namespace Reporting.Core.Contracts
{
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using System.Data;

    public interface IDapperConnectionService
    {
        IDbConnection GetConnection();
    }
}
