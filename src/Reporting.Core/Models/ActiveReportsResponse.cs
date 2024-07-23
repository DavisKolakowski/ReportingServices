namespace Reporting.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ActiveReportsResponse
    {
        public IEnumerable<ActiveReportModel> Model { get; set; } = new List<ActiveReportModel>();
    }
}
