namespace Reporting.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class DownloadReportRequest
    {
        public ReportDetailsModel Model { get; set; } = new ReportDetailsModel();
    }
}
