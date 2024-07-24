namespace Reporting.Core.Models
{
    public class ReportDetailsModel
    {
        public string? Key { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public bool HasParameters { get; set; }
        public int ReportSourceId { get; set; }
        public IEnumerable<ReportParameterModel>? Parameters { get; set; }
        public IEnumerable<ReportColumnDefinitionModel> ColumnDefinitions { get; set; } = Array.Empty<ReportColumnDefinitionModel>();
        public string? CreatedByUser { get; set; }
        public DateTime CreatedAtDate { get; set; }
        public string? LastUpdatedByUser { get; set; } = null;
        public DateTime? LastUpdatedAtDate { get; set; } = null;
    }
}
