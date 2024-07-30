namespace Reporting.Core.Models
{
    using Reporting.Core.Helpers;

    public class ReportParameterModel
    {
        private object? _currentValue;

        public int Position { get; set; }
        public string? Name { get; set; }
        public string? SqlDataType { get; set; }
        public bool HasDefaultValue { get; set; }
        public object? CurrentValue
        {
            get => _currentValue;
            set => _currentValue = ObjectHelpers.ConvertSqlValue(value, SqlDataType);
        }
    }
}
