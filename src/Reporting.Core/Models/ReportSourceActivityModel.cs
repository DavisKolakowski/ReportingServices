namespace Reporting.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Reporting.Core.Enums;

    public class ReportSourceActivityModel
    {
        public ActivityType ActivityType { get; set; }
        public string? ActivityByUser { get; set; }
        public DateTime ActivityDate { get; set; }
    }
}
