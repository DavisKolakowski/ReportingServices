namespace Reporting.Core.Models
{
    public class ReportColumnDefinitionModel
    {
        public int ColumnId { get; set; }
        public string? Name { get; set; }
        public string? SqlDataType { get; set; }
        public bool IsNullable { get; set; }
        public bool IsIdentity { get; set; }
    }
}
