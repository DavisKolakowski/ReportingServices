namespace Reporting.Core.Helpers
{
    using System;
    using System.Text.Json;
    using DocumentFormat.OpenXml.Spreadsheet;

    public static class ObjectHelpers
    {
        public static object? ConvertSqlValue(object? value, string sqlDataType)
        {
            if (value == null)
            {
                return null;
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

        public static Cell CreateCell(object? value, string sqlDataType)
        {
            if (value == null)
            {
                return new Cell() { CellValue = new CellValue(string.Empty), DataType = CellValues.String };
            }

            var cellValue = ConvertSqlValue(value, sqlDataType);
            return cellValue switch
            {
                string stringValue => new Cell() { CellValue = new CellValue(stringValue), DataType = CellValues.String },
                int intValue => new Cell() { CellValue = new CellValue(intValue.ToString()), DataType = CellValues.Number },
                double doubleValue => new Cell() { CellValue = new CellValue(doubleValue.ToString()), DataType = CellValues.Number },
                bool boolValue => new Cell() { CellValue = new CellValue(boolValue ? "1" : "0"), DataType = CellValues.Boolean },
                DateTime dateTimeValue => new Cell() { CellValue = new CellValue(dateTimeValue.ToOADate().ToString()), DataType = CellValues.Date },
                Guid guidValue => new Cell() { CellValue = new CellValue(guidValue.ToString()), DataType = CellValues.String },
                TimeSpan timeSpanValue => new Cell() { CellValue = new CellValue(timeSpanValue.ToString()), DataType = CellValues.String },
                _ => new Cell() { CellValue = new CellValue(cellValue?.ToString() ?? string.Empty), DataType = CellValues.String }
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
