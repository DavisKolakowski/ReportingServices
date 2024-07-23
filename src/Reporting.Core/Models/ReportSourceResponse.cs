namespace Reporting.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ReportSourceResponse
    {
        public required ReportSourceModel Model { get; set; }

        public IEnumerable<ReportSourceActivityModel> ActivityLog { get; set; } = new List<ReportSourceActivityModel>();
    }
}
