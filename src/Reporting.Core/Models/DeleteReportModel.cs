﻿namespace Reporting.Core.Models
{
    using System.ComponentModel.DataAnnotations;

    public class DeleteReportModel
    {
        [Required]
        [RegularExpression(@"^[a-z0-9_]+$", ErrorMessage = "Key can only contain lowercase letters, numbers, and underscores.")]
        public string? Key { get; set; }
    }
}
