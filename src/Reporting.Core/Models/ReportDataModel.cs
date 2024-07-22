using Reporting.Core.Entities;

namespace Reporting.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ReportDataModel
    {
        public IEnumerable<Dictionary<string, object>>? Data { get; set; } = new List<Dictionary<string, object>>();
    }
}
