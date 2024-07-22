namespace Reporting.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class DataTableExtensions
    {
        public static IEnumerable<Dictionary<string, object>> ToDictionaryList(this DataTable dataTable)
        {
            return dataTable.AsEnumerable().Select(row =>
            {
                var dict = new Dictionary<string, object>();
                foreach (DataColumn column in dataTable.Columns)
                {
                    dict[column.ColumnName] = row[column] == DBNull.Value ? DBNull.Value : row[column];
                }
                return dict;
            });
        }
    }
}
