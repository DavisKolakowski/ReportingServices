namespace Reporting.Core.Entities
{
    using System;

    public class Report
    {
        public int Id { get; set; }
        public string? Key { get; set; } //unique string key for the report can only contain lowercase letters, numbers, and underscores
        public int ReportSourceId { get; set; } //unique only one report can exist for a source       
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? CreatedByUser { get; set; }
        public DateTime CreatedAtDate { get; set; }
        public string? UpdatedByUser { get; set; }
        public DateTime? UpdatedAtDate { get; set; }
        public bool IsActive { get; set; }
        public bool HasParameters { get; set; }
        public ReportParameter[]? Parameters { get; set; } //null if HasParameters is false
        public ReportColumnDefinition[] ColumnDefinitions { get; set; } = Array.Empty<ReportColumnDefinition>();
    }
}
