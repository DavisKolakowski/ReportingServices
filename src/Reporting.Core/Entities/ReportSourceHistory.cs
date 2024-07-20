namespace Reporting.Core.Entities
{
    using Reporting.Core.Enums;

    public class ReportSourceHistory //this entity should be managed by the database trigger.
    {
        public int Id { get; set; }
        public required ReportSourceType Type { get; set; } // View or Procedure
        public required string SqlObjectName { get; set; } // report source full name
        public required ActivityType ActivityType { get; set; } // Created, Altered, Deleted
        public required string ActivityByUser { get; set; } //example [domain\user] by using SUSER_NAME()
        public required DateTime ActivityDate { get; set; }
    }
}
