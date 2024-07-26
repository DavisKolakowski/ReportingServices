namespace Reporting.Core.Models
{
    public class ReportDetailsModel
    {
        public ReportModel? Report { get; set; }
        public IEnumerable<ReportParameterModel>? Parameters { get; set; }
        public IEnumerable<ReportColumnDefinitionModel> ColumnDefinitions { get; set; } = Array.Empty<ReportColumnDefinitionModel>();
    }
}
