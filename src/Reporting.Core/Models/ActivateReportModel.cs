namespace Reporting.Core.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ActivateReportModel
    {
        [Required]
        [RegularExpression(@"^[a-z0-9_]+$", ErrorMessage = "Key can only contain lowercase letters, numbers, and underscores.")]
        public string? Key { get; set; }

        [Required]
        public string? UpdatedByUser { get; set; }

        [Required]
        public DateTime UpdatedAtDate { get; set; } = DateTime.UtcNow;
    }
}
