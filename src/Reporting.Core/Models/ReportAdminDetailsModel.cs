namespace Reporting.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ReportAdminDetailsModel
    {
        public string? Key { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public bool HasParameters { get; set; }
        public int ReportSourceId { get; set; }
        public ReportSourceModel? ReportSource { get; set; }
        public ReportParameterModel[]? Parameters { get; set; }
        public ReportColumnDefinitionModel[] ColumnDefinitions { get; set; } = Array.Empty<ReportColumnDefinitionModel>();
        public string? CreatedByUser { get; set; }
        public DateTime CreatedAtDate { get; set; }
        public string? LastUpdatedByUser { get; set; } = null;
        public DateTime? LastUpdatedAtDate { get; set; } = null;
    }
}
