namespace Reporting.Core.Contracts
{
    using Reporting.Core.Entities;

    public interface ISystemRepository
    {
        Task<IEnumerable<ReportParameter>> GetReportParametersAsync(string sqlObjectName);
        Task<IEnumerable<ReportColumnDefinition>> GetReportColumnDefinitionsAsync(string sqlObjectName);
    }
}
