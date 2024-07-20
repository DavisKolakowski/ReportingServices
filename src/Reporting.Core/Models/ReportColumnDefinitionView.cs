namespace Reporting.Core.Models
{
    public class ReportColumnDefinitionView
    {
        public int Position { get; set; }
        public string? Name { get; set; }
        public string? SqlDataType { get; set; }
        public bool IsNullable { get; set; }
        public bool IsIdentity { get; set; }
    }
}
