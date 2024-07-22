namespace Reporting.Core.Models
{
    using System.Collections.Specialized;
    using System.Data;

    public class ExecuteReportResponse
    {
        public ReportDetailsModel Model { get; set; } = new ReportDetailsModel();
        public IEnumerable<Dictionary<string, object>>? Data { get; set; } = new List<Dictionary<string, object>>();
    }
}
