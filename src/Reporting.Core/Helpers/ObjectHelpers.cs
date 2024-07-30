namespace Reporting.Core.Helpers
{
    using System;
    using System.Data;
    using System.Text.Json;
    using ClosedXML.Excel;

    using DocumentFormat.OpenXml.Spreadsheet;

    public static class ObjectHelpers
    {
        public static object? ConvertSqlValue(object? value, string? sqlDataType)
        {
            if (value == null || string.IsNullOrEmpty(sqlDataType))
            {
                return DBNull.Value;
            }

            var convertedValue = FormatValue(value);

            return sqlDataType.ToLower() switch
            {
                "tinyint" => Convert.ToByte(convertedValue),
                "smallint" => Convert.ToInt16(convertedValue),
                "int" => Convert.ToInt32(convertedValue),
                "bigint" => Convert.ToInt64(convertedValue),
                "bit" => Convert.ToBoolean(convertedValue),
                "decimal" => Convert.ToDecimal(convertedValue),
                "numeric" => Convert.ToDecimal(convertedValue),
                "money" => Convert.ToDecimal(convertedValue),
                "smallmoney" => Convert.ToDecimal(convertedValue),
                "float" => Convert.ToDouble(convertedValue),
                "real" => Convert.ToSingle(convertedValue),
                "date" => Convert.ToDateTime(convertedValue),
                "datetime" => Convert.ToDateTime(convertedValue),
                "smalldatetime" => Convert.ToDateTime(convertedValue),
                "datetime2" => Convert.ToDateTime(convertedValue),
                "time" => TimeSpan.TryParse(convertedValue?.ToString(), out var timeResult) ? timeResult : TimeSpan.Zero,
                "datetimeoffset" => DateTimeOffset.TryParse(convertedValue?.ToString(), out var dtoResult) ? dtoResult : DateTimeOffset.MinValue,
                "char" => Convert.ToString(convertedValue) ?? string.Empty,
                "varchar" => Convert.ToString(convertedValue) ?? string.Empty,
                "text" => Convert.ToString(convertedValue) ?? string.Empty,
                "nchar" => Convert.ToString(convertedValue) ?? string.Empty,
                "nvarchar" => Convert.ToString(convertedValue) ?? string.Empty,
                "ntext" => Convert.ToString(convertedValue) ?? string.Empty,
                "binary" => convertedValue is byte[] bytes ? bytes : Array.Empty<byte>(),
                "varbinary" => convertedValue is byte[] varBytes ? varBytes : Array.Empty<byte>(),
                "image" => convertedValue is byte[] imgBytes ? imgBytes : Array.Empty<byte>(),
                "uniqueidentifier" => Guid.TryParse(convertedValue?.ToString(), out var guidResult) ? guidResult : Guid.Empty,
                _ => convertedValue
            };
        }

        public static Type GetCSharpTypeForSqlTypeString(string sqlType)
        {
            return sqlType.ToLower() switch
            {
                "bigint" => typeof(long),
                "binary" => typeof(byte[]),
                "bit" => typeof(bool),
                "char" => typeof(string),
                "date" => typeof(DateTime),
                "datetime" => typeof(DateTime),
                "datetime2" => typeof(DateTime),
                "datetimeoffset" => typeof(DateTimeOffset),
                "decimal" => typeof(decimal),
                "float" => typeof(double),
                "image" => typeof(byte[]),
                "int" => typeof(int),
                "money" => typeof(decimal),
                "nchar" => typeof(string),
                "ntext" => typeof(string),
                "numeric" => typeof(decimal),
                "nvarchar" => typeof(string),
                "real" => typeof(float),
                "smalldatetime" => typeof(DateTime),
                "smallint" => typeof(short),
                "smallmoney" => typeof(decimal),
                "text" => typeof(string),
                "time" => typeof(TimeSpan),
                "timestamp" => typeof(byte[]),
                "tinyint" => typeof(byte),
                "uniqueidentifier" => typeof(Guid),
                "varbinary" => typeof(byte[]),
                "varchar" => typeof(string),
                _ => typeof(object),
            };
        }

        private static object? FormatValue(object value)
        {
            var jsonElement = JsonSerializer.SerializeToElement(value);
            return jsonElement.ValueKind switch
            {
                JsonValueKind.Object => jsonElement,
                JsonValueKind.Array => jsonElement,
                JsonValueKind.String => jsonElement.GetString(),
                JsonValueKind.Number => jsonElement.TryGetInt32(out var intValue)
                    ? intValue : jsonElement.TryGetDouble(out var doubleValue)
                    ? doubleValue : (object?)jsonElement.GetDecimal(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => null,
                JsonValueKind.Undefined => null,
                _ => null
            };
        }
    }
}
