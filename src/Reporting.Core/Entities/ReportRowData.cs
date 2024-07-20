namespace Reporting.Core.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ReportRowData
    {
        public Dictionary<string, object> ColumnKeys { get; set; } = new Dictionary<string, object>();
    }
}
