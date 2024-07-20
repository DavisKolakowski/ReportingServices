namespace Reporting.Core.Models
{
    using System.ComponentModel.DataAnnotations;

    public class DisableReportModel
    {
        [Required]
        [RegularExpression(@"^[a-z0-9_]+$", ErrorMessage = "Key can only contain lowercase letters, numbers, and underscores.")]
        public required string Key { get; set; }

        [Required]
        public required string UpdatedByUser { get; set; }

        [Required]
        public DateTime UpdatedAtDate { get; set; } = DateTime.UtcNow;
    }
}
