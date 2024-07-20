namespace Reporting.Core.Models
{
    public class ReportDetailsModel
    {
        public string? Key { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public bool HasParameters { get; set; }
        public ReportParameterView[]? Parameters { get; set; }
        public ReportColumnDefinitionView[] ColumnDefinitions { get; set; } = Array.Empty<ReportColumnDefinitionView>();
    }
}
