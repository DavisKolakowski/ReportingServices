namespace Reporting.Core.Entities
{
    using Reporting.Core.Enums;

    public class ReportSourceHistory //this entity should be managed by the database trigger.
    {
        public int Id { get; set; }
        public ReportSourceType Type { get; set; } // View or Procedure
        public string? SqlObjectName { get; set; } // report source full name
        public ActivityType ActivityType { get; set; } // Created, Altered, Deleted
        public string? ActivityByUser { get; set; } //example [domain\user] by using SUSER_NAME()
        public DateTime ActivityDate { get; set; }
    }
}
