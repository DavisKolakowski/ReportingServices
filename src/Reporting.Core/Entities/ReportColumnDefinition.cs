namespace Reporting.Core.Entities
{

    public class ReportColumnDefinition
    {
        public int ColumnId { get; set; } // This is the column order by id that is used in the database system tables

        public string? Name { get; set; }

        public string? SqlDataType { get; set; }

        public bool IsNullable { get; set; }

        public bool IsIdentity { get; set; } // If the name of the report column is IndernalId, then it is an identity column
    }
}
