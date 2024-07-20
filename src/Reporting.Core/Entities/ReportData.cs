namespace Reporting.Core.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ReportData
    {
        public IEnumerable<ReportColumnDefinition> ColumnDefinitions { get; set; } = new List<ReportColumnDefinition>();
        public IEnumerable<ReportRowData>? Data { get; set; }
    }
}
