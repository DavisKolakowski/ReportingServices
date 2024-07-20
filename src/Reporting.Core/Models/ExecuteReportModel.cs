namespace Reporting.Core.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ExecuteReportModel
    {
        [Required]
        [RegularExpression(@"^[a-z0-9_]+$", ErrorMessage = "Key can only contain lowercase letters, numbers, and underscores.")]
        public required string Key { get; set; }

        public Dictionary<string, object>? Parameters { get; set; }
    }
}
