namespace Reporting.Core.Models
{
    using System.Collections.Generic;

    public class ReportSourceResponse
    {
        public ReportSourceModel Model { get; set; } = new ReportSourceModel();

        public IEnumerable<ReportSourceActivityModel> ActivityLog { get; set; } = new List<ReportSourceActivityModel>();
    }
}
