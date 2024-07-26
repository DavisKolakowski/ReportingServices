namespace Reporting.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ReportParametersResponse
    {
        public string ReportKey { get; set; } = string.Empty;
        public IEnumerable<ReportParameterModel> Model { get; set; } = Array.Empty<ReportParameterModel>();
    }
}
