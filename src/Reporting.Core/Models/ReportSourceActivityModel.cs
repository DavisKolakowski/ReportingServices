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
        public required ActivityType ActivityType { get; set; }
        public required string ActivityByUser { get; set; }
        public required DateTime ActivityDate { get; set; }
    }
}
