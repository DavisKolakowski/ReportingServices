namespace Reporting.Core.Models
{
    public class ReportDataModel : ReportDetailsModel
    {
        public ReportDataModel(ReportDetailsModel meta) : base()
        { 
            base.Report = meta.Report;
            base.Parameters = meta.Parameters;
            base.ColumnDefinitions = meta.ColumnDefinitions;
        }

        public IEnumerable<Dictionary<string, object>> Data { get; set; } = new List<Dictionary<string, object>>();
    }
}
