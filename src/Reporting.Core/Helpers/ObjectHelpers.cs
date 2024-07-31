namespace Reporting.Core.Helpers
{
    using System;
    using System.Data;
    using System.Text.Json;

    using Dapper;

    public static class ObjectHelpers
    {
        public static object? ConvertSqlValue(object? value, string? sqlDataType)
        {
            if (value == null || string.IsNullOrEmpty(sqlDataType))
            {
                return DBNull.Value;
            }

            var convertedValue = ConvertJsonElement(value);

            if (convertedValue == null)
            {
                return DBNull.Value;
            }

            switch (sqlDataType.ToLower())
            {
                case "tinyint":
                    return Convert.ToByte(convertedValue);
                case "smallint":
                    return Convert.ToInt16(convertedValue);
                case "int":
                    return Convert.ToInt32(convertedValue);
                case "bigint":
                    return Convert.ToInt64(convertedValue);
                case "bit":
                    return Convert.ToBoolean(convertedValue);
                case "decimal":
                case "numeric":
                case "money":
                case "smallmoney":
                    return Convert.ToDecimal(convertedValue);
                case "float":
                    return Convert.ToDouble(convertedValue);
                case "real":
                    return Convert.ToSingle(convertedValue);
                case "date":
                case "datetime":
                case "smalldatetime":
                case "datetime2":
                    return DateTime.TryParse(convertedValue.ToString(), out var dateTimeValue) ? dateTimeValue : throw new InvalidCastException($"Unable to cast {convertedValue} to DateTime.");
                case "time":
                    return TimeSpan.TryParse(convertedValue?.ToString(), out var timeResult) ? timeResult : TimeSpan.Zero;
                case "datetimeoffset":
                    return DateTimeOffset.TryParse(convertedValue?.ToString(), out var dtoResult) ? dtoResult : DateTimeOffset.MinValue;
                case "char":
                case "varchar":
                case "text":
                case "nchar":
                case "nvarchar":
                case "ntext":
                    return Convert.ToString(convertedValue) ?? string.Empty;
                case "binary":
                    return convertedValue is byte[] bytes ? bytes : Array.Empty<byte>();
                case "varbinary":
                    return convertedValue is byte[] varBytes ? varBytes : Array.Empty<byte>();
                case "image":
                    return convertedValue is byte[] imgBytes ? imgBytes : Array.Empty<byte>();
                case "uniqueidentifier":
                    return Guid.TryParse(convertedValue.ToString(), out var guidResult) ? guidResult : Guid.Empty;
                default:
                    return convertedValue;
            }
        }

        public static Type GetCSharpTypeForSqlTypeString(string sqlType)
        {
            switch (sqlType.ToLower())
            {
                case "bigint":
                    return typeof(long);
                case "binary":
                    return typeof(byte[]);
                case "bit":
                    return typeof(bool);
                case "char":
                    return typeof(string);
                case "date":
                case "datetime":
                case "datetime2":
                    return typeof(DateTime);
                case "datetimeoffset":
                    return typeof(DateTimeOffset);
                case "decimal":
                case "numeric":
                    return typeof(decimal);
                case "float":
                    return typeof(double);
                case "image":
                    return typeof(byte[]);
                case "int":
                    return typeof(int);
                case "money":
                    return typeof(decimal);
                case "nchar":
                case "ntext":
                case "nvarchar":
                case "varchar":
                case "text":
                    return typeof(string);
                case "real":
                    return typeof(float);
                case "smalldatetime":
                    return typeof(DateTime);
                case "smallint":
                    return typeof(short);
                case "smallmoney":
                    return typeof(decimal);
                case "time":
                    return typeof(TimeSpan);
                case "timestamp":
                    return typeof(byte[]);
                case "tinyint":
                    return typeof(byte);
                case "uniqueidentifier":
                    return typeof(Guid);
                case "varbinary":
                    return typeof(byte[]);
                default:
                    return typeof(object);
            }
        }

        public static object ConvertToObject(string value)
        {
            try
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>(value);
                return ConvertJsonElement(jsonElement)!;
            }
            catch
            {
                return value;
            }
        }

        private static object? ConvertJsonElement(object value)
        {
            if (value is JsonElement jsonElement)
            {
                return jsonElement.ValueKind switch
                {
                    JsonValueKind.String => jsonElement.GetString(),
                    JsonValueKind.Number => jsonElement.TryGetInt32(out var intValue) ? intValue :
                                            jsonElement.TryGetInt64(out var longValue) ? longValue :
                                            jsonElement.TryGetDecimal(out var decimalValue) ? decimalValue :
                                            jsonElement.TryGetDouble(out var doubleValue) ? doubleValue : jsonElement.GetDecimal(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    JsonValueKind.Null => DBNull.Value,
                    _ => jsonElement.GetRawText(),
                };
            }

            if (value.GetType() == typeof(object) && value.ToString() == "{}")
            {
                return DBNull.Value;
            }

            return value;
        }
    }
}
