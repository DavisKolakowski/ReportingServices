namespace Reporting.Core.Models
{
    public class ExecuteReportResponse
    {
        public ExecuteReportResponse(ReportDataModel dataModel)
        {
            if (dataModel == null)
            {
                throw new ArgumentNullException(nameof(dataModel));
            }
            this.Data = dataModel.Data;
        }
        public ReportDetailsModel Model { get; set; } = new ReportDetailsModel();
        public IEnumerable<Dictionary<string, object>>? Data { get; private set; } = new List<Dictionary<string, object>>();
    }
}
