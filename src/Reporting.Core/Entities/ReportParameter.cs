namespace Reporting.Core.Entities
{
    using Reporting.Core.Helpers;

    public class ReportParameter
    {
        public int Position { get; set; }
        public required string Name { get; set; }

        private object? _currentValue;
        public object? CurrentValue
        {
            get => _currentValue;
            set => _currentValue = ObjectHelpers.ConvertSqlValue(value ?? new { }, SqlDataType);
        }

        public string? SqlDataType { get; set; }
        public bool HasDefaultValue { get; set; }
    }
}
