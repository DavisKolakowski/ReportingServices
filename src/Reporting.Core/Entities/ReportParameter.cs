namespace Reporting.Core.Entities
{
    using Reporting.Core.Helpers;

    public class ReportParameter
    {
        public required int Position { get; set; }
        public required string Name { get; set; }

        private object? _currentValue;
        public object? CurrentValue
        {
            get => _currentValue;
            set => _currentValue = ObjectHelpers.ConvertSqlValue(value ?? new { }, SqlDataType);
        }

        public required string SqlDataType { get; set; }
        public bool HasDefaultValue { get; set; }
    }
}
