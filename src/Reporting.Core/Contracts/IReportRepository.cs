namespace Reporting.Core.Contracts
{
    using Reporting.Core.Entities;

    public interface IReportRepository
    {
        Task<IEnumerable<Report>> GetAllAsync();
        Task<IEnumerable<Report>> GetAllActiveAsync();
        Task<Report?> GetByKeyAsync(string reportKey);
        Task<Report?> GetByIdAsync(int reportId);
        Task<ReportData> ExecuteAsync(ReportSource source, ReportColumnDefinition[] columns, ReportParameter[]? parameters = null);
        Task<Report> CreateAsync(Report report);
        Task<Report> UpdateAsync(Report report);
        Task DeleteAsync(Report report);
    }
}
