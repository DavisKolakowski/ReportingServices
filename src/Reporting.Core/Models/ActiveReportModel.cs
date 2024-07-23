namespace Reporting.Core.Models
{
    public class ActiveReportModel
    {
        public string? Key { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool HasParameters { get; set; }
        public string? CreatedByUser { get; set; }
        public DateTime CreatedAtDate { get; set; }
        public string? LastUpdatedByUser { get; set; } = null;
        public DateTime? LastUpdatedAtDate { get; set; } = null;
    }
}
