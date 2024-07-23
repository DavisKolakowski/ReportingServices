namespace Reporting.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class AvailableReportSourcesForNewReportResponse
    {
        public IEnumerable<ReportSourceModel> Model { get; set; } = new List<ReportSourceModel>();
    }
}
