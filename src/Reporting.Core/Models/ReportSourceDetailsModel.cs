namespace Reporting.Core.Models
{
    using System.Collections.Generic;

    public class ReportSourceDetailsModel
    {
        public ReportSourceModel Source { get; set; } = new ReportSourceModel();

        public IEnumerable<ReportSourceActivityModel> ActivityLog { get; set; } = new List<ReportSourceActivityModel>();
    }
}
