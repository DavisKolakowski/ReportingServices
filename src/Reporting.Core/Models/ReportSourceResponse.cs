namespace Reporting.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ReportSourceResponse
    {
        public ReportSourceModel Model { get; set; } = new ReportSourceModel();

        public IEnumerable<ReportSourceActivityModel> ActivityLog { get; set; } = new List<ReportSourceActivityModel>();
    }
}
