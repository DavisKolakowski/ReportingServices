namespace Reporting.Core.Entities
{
    using Reporting.Core.Enums;

    public class ReportSource //this entity should be managed by the database trigger.
    {
        public int Id { get; set; }
        public required ReportSourceType Type { get; set; } // View or Procedure
        public required string Schema { get; set; } //example [Reporting]
        public required string Name { get; set; } //example [ReportName]
        public string FullName => $"{Schema}.{Name}"; //example [Reporting].[ReportName]
        public required ActivityType LastActivityType { get; set; } // Created, Altered, Deleted
        public required string LastActivityByUser { get; set; } //example [domain\user] by using SUSER_NAME()
        public required DateTime LastActivityDate { get; set; }
    }
}
