﻿namespace Reporting.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class NewReportModel
    {
        [Required]
        [RegularExpression(@"^[a-z0-9_]+$", ErrorMessage = "Key can only contain lowercase letters, numbers, and underscores.")]
        public string? Key { get; set; }

        [Required]
        public int ReportSourceId { get; set; }

        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        [Required]
        public string? CreatedByUser { get; set; }

        [Required]
        public DateTime CreatedAtDate { get; set; } = DateTime.UtcNow;
    }
}
