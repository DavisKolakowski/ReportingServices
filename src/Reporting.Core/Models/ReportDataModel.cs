namespace Reporting.Core.Models
{
    public class ReportDataModel : ReportDetailsModel
    {
        public IEnumerable<Dictionary<string, object>> Data { get; set; } = new List<Dictionary<string, object>>();
    }
}
