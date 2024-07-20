namespace Reporting.Core.Models
{
    public class ReportParameterView
    {
        public int Position { get; set; }
        public string? Name { get; set; }
        public string? SqlDataType { get; set; }
        public bool HasDefaultValue { get; set; }
        public object? CurrentValue { get; set; }
    }
}
