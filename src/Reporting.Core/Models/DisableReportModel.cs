namespace Reporting.Core.Models
{
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Http;

    public class DisableReportModel
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
