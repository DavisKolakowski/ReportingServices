namespace Reporting.Core.Entities
{
    using System;

    public class Report
    {
        public int Id { get; set; }
        public required string Key { get; set; } //unique string key for the report can only contain lowercase letters, numbers, and underscores
        public required int ReportSourceId { get; set; } //unique only one report can exist for a source       
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string CreatedByUser { get; set; }
        public required DateTime CreatedAtDate { get; set; }
        public string? UpdatedByUser { get; set; }
        public DateTime? UpdatedAtDate { get; set; }
        public required bool IsActive { get; set; }
        public required bool HasParameters { get; set; }
        public ReportParameter[]? Parameters { get; set; } //null if HasParameters is false
        public ReportColumnDefinition[] ColumnDefinitions { get; set; } = Array.Empty<ReportColumnDefinition>();
    }
}
