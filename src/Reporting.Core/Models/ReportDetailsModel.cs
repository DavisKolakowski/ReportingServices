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
        public ReportParameterModel[]? Parameters { get; set; }
        public ReportColumnDefinitionModel[] ColumnDefinitions { get; set; } = Array.Empty<ReportColumnDefinitionModel>();
    }
}
