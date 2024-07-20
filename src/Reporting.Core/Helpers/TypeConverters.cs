namespace Reporting.Core.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public static class TypeConverters
    {
        public static object? ConvertSqlValue(object? value, string sqlDataType)
        {
            if (value == null)
            {
                return null;
            }

            var jsonElement = System.Text.Json.JsonSerializer.SerializeToElement(value);
            return jsonElement.ValueKind switch
            {
                JsonValueKind.String => jsonElement.GetString(),
                JsonValueKind.Number => jsonElement.TryGetInt32(out var intValue) ? intValue : (jsonElement.TryGetDouble(out var doubleValue) ? doubleValue : jsonElement.GetDecimal()),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => null,
                JsonValueKind.Undefined => null,
                JsonValueKind.Object => jsonElement,
                JsonValueKind.Array => jsonElement,
                _ => ConvertSqlToCSharp(value, sqlDataType)
            };
        }

        private static object ConvertSqlToCSharp(object value, string sqlDataType)
        {
            return sqlDataType.ToLower() switch
            {
                "tinyint" => Convert.ToByte(value),
                "smallint" => Convert.ToInt16(value),
                "int" => Convert.ToInt32(value),
                "bigint" => Convert.ToInt64(value),
                "bit" => Convert.ToBoolean(value),
                "decimal" => Convert.ToDecimal(value),
                "numeric" => Convert.ToDecimal(value),
                "money" => Convert.ToDecimal(value),
                "smallmoney" => Convert.ToDecimal(value),
                "float" => Convert.ToDouble(value),
                "real" => Convert.ToSingle(value),
                "date" => Convert.ToDateTime(value),
                "datetime" => Convert.ToDateTime(value),
                "smalldatetime" => Convert.ToDateTime(value),
                "datetime2" => Convert.ToDateTime(value),
                "time" => TimeSpan.TryParse(value?.ToString(), out var timeResult) ? timeResult : TimeSpan.Zero,
                "datetimeoffset" => DateTimeOffset.TryParse(value?.ToString(), out var dtoResult) ? dtoResult : DateTimeOffset.MinValue,
                "char" => Convert.ToString(value) ?? string.Empty,
                "varchar" => Convert.ToString(value) ?? string.Empty,
                "text" => Convert.ToString(value) ?? string.Empty,
                "nchar" => Convert.ToString(value) ?? string.Empty,
                "nvarchar" => Convert.ToString(value) ?? string.Empty,
                "ntext" => Convert.ToString(value) ?? string.Empty,
                "binary" => value is byte[] bytes ? bytes : Array.Empty<byte>(),
                "varbinary" => value is byte[] varBytes ? varBytes : Array.Empty<byte>(),
                "image" => value is byte[] imgBytes ? imgBytes : Array.Empty<byte>(),
                "uniqueidentifier" => Guid.TryParse(value?.ToString(), out var guidResult) ? guidResult : Guid.Empty,
                _ => value
            };
        }
    }
}
