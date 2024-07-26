namespace Reporting.Core.Models
{
    using System.Collections.Generic;

    public class AvailableReportSourcesForNewReportResponse
    {
        public IEnumerable<ReportSourceModel> Model { get; set; } = new List<ReportSourceModel>();
    }
}
