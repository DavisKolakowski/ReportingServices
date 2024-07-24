namespace Reporting.Core.Entities
{
    using Reporting.Core.Enums;

    public class ReportSource //this entity should be managed by the database trigger.
    {
        public int Id { get; set; }
        public ReportSourceType Type { get; set; } // View or Procedure
        public string? Schema { get; set; } //example [Reporting]
        public string? Name { get; set; } //example [ReportName]
        public string FullName => $"{Schema}.{Name}"; //example [Reporting].[ReportName]
        public ActivityType LastActivityType { get; set; } // Created, Altered, Deleted
        public string? LastActivityByUser { get; set; } //example [domain\user] by using SUSER_NAME()
        public DateTime LastActivityDate { get; set; }
    }
}
