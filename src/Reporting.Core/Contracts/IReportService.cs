namespace Reporting.Core.Contracts
{
    using System.Data;

    using Reporting.Core.Entities;
    using Reporting.Core.Models;

    public interface IReportService
    {
        Task<IEnumerable<Report>> GetAllReportsAsync();
        Task<IEnumerable<Report>> GetAllActiveReportsAsync();
        Task<IEnumerable<ReportSource>> GetAllAvailableReportSourcesAsync();
        Task<Report?> GetReportByKeyAsync(string reportKey);
        Task<IEnumerable<ReportParameter>> GetParametersForReportKeyAsync(string reportKey);
        Task<IEnumerable<ReportColumnDefinition>> GetColumnDefinitionsForReportKeyAsync(string reportKey);
        Task<Report?> GetReportDetailsAsync(string reportKey);
        Task<ReportSource> GetReportSourceAsync(Report report);
        Task<ReportSource> GetReportSourceByIdAsync(int reportSourceId);
        Task<IEnumerable<ReportSourceHistory>> GetReportSourceActivityLogAsync(string sqlObjectName);
        Task<Report> CreateReportAsync(NewReportModel report);
        Task<Report> UpdateReportAsync(UpdateReportModel report);
        Task DeleteReportAsync(DeleteReportModel report);
        Task<IEnumerable<Dictionary<string, object>>> GetReportDataAsTableAsync(ReportDetailsModel report);
        Task<byte[]> GetReportDataAsBytesAsync(ReportDetailsModel report);
    }
}
