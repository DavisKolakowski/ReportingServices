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
        public required int Id { get; set; }
        public required ReportSourceType Type { get; set; }
        public required string Schema { get; set; }
        public required string Name { get; set; }
        public string FullName => $"{Schema}.{Name}";
        public required ActivityType LastActivityType { get; set; }
        public required string LastActivityByUser { get; set; }
        public required DateTime LastActivityDate { get; set; }
    }
}
