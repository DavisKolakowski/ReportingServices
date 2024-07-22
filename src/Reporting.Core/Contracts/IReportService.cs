namespace Reporting.Core.Contracts
{
    using Reporting.Core.Entities;
    using Reporting.Core.Models;

    public interface IReportService
    {
        Task<IEnumerable<Report>> GetAllReportsAsync();
        Task<IEnumerable<Report>> GetAllActiveReportsAsync();
        Task<IEnumerable<ReportSource>> GetAllAvailableReportSourcesAsync();
        Task<Report?> GetReportByKeyAsync(string reportKey);
        Task<Report?> GetReportDetailsAsync(string reportKey);
        Task<IEnumerable<ReportSourceHistory>> GetReportActivityAsync(string reportKey);
        Task<Report> CreateReportAsync(NewReportModel report);
        Task<Report> UpdateReportAsync(UpdateReportModel report);
        Task ActivateReportAsync(ActivateReportModel report);
        Task DisableReportAsync(DisableReportModel report);
        Task DeleteReportAsync(DeleteReportModel report);
        Task<ReportDataModel> GetReportDataGridAsync(ReportDetailsModel report);
        Task<byte[]> GetReportDataAsBytesAsync(ReportDetailsModel report);
    }
}
