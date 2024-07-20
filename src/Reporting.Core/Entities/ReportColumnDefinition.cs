namespace Reporting.Core.Entities
{

    public class ReportColumnDefinition
    {
        public required int Position { get; set; } // This is the column order by id that is used in the database system tables

        public required string Name { get; set; }

        public required string SqlDataType { get; set; }

        public bool IsNullable { get; set; }

        public bool IsIdentity { get; set; } // If the name of the report column is IndernalId, then it is an identity column
    }
}
