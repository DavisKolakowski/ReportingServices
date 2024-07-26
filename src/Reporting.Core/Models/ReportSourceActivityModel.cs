namespace Reporting.Core.Models
{
    using System;
    using Reporting.Core.Enums;

    public class ReportSourceActivityModel
    {
        public ActivityType ActivityType { get; set; }
        public string? ActivityByUser { get; set; }
        public DateTime ActivityDate { get; set; }
    }
}
