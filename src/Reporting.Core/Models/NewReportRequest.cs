namespace Reporting.Core.Models
{
    using System.ComponentModel.DataAnnotations;

    public class NewReportRequest
    {
        [Required]
        [RegularExpression(@"^[a-z0-9_]+$", ErrorMessage = "Key can only contain lowercase letters, numbers, and underscores.")]
        public required string Key { get; set; }

        [Required]
        public int ReportSourceId { get; set; }

        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}
