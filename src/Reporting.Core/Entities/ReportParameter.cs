namespace Reporting.Core.Entities
{
    using Reporting.Core.Helpers;

    public class ReportParameter
    {
        public int Position { get; set; }
        public required string Name { get; set; }
        public object? CurrentValue { get; set; }
        public string? SqlDataType { get; set; }
        public bool HasDefaultValue { get; set; }
    }
}
