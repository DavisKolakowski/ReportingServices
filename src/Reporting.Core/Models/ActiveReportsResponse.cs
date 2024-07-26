namespace Reporting.Core.Models
{
    using System.Collections.Generic;

    public class ActiveReportsResponse
    {
        public IEnumerable<ActiveReportModel> Model { get; set; } = new List<ActiveReportModel>();
    }
}
