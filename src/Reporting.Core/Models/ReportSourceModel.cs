namespace Reporting.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Reporting.Core.Enums;

    public class ReportSourceModel
    {
        public int Id { get; set; }
        public ReportSourceType Type { get; set; }
        public string? Schema { get; set; }
        public string? Name { get; set; }
        public string FullName => $"{Schema}.{Name}";
        public ActivityType LastActivityType { get; set; }
        public string? LastActivityByUser { get; set; }
        public DateTime LastActivityDate { get; set; }
    }
}
