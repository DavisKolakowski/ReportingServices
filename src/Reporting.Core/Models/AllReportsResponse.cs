namespace Reporting.Core.Models
{
    using System.Collections.Generic;

    public class AllReportsResponse
    {
        public IEnumerable<ReportModel> Model { get; set; } = new List<ReportModel>();
    }
}
