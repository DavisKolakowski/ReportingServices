namespace Reporting.Core.Models
{
    using Reporting.Core.Helpers;

    public class ReportParameterModel
    {
        public int Position { get; set; }
        public string? Name { get; set; }
        public string? SqlDataType { get; set; }
        public bool HasDefaultValue { get; set; }
        public object? CurrentValue { get; set; }
    }
}
