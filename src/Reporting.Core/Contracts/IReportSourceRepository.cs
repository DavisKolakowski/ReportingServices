namespace Reporting.Core.Contracts
{
    using Reporting.Core.Entities;

    public interface IReportSourceRepository
    {
        Task<ReportSource?> GetByIdAsync(int reportSourceId);
        Task<IEnumerable<ReportSource>> GetUnusedAsync();
        Task<IEnumerable<ReportSourceHistory>> GetActivityHistoryAsync(string sqlObjectName);
    }
}
