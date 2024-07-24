namespace Reporting.Core.Utilities
{
    using System;
    using System.Data;
    using System.IO;
    using System.Linq;

    using ClosedXML.Excel;

    using Reporting.Core.Entities;
    using Reporting.Core.Helpers;
    using Reporting.Core.Models;

    public static class ReportWorkbookUtility
    {
        const string RowIdentifierKey = "InternalId";

        public static byte[] CreateExcelReport(ReportDetailsModel report, DataTable dataTable)
        {
            var parameters = report.Parameters;
            var columns = report.ColumnDefinitions.AsEnumerable();

            if (dataTable == null)
            {
                throw new ArgumentNullException(nameof(dataTable), "Data cannot be null when creating an Excel report.");
            }

            using (var memoryStream = new MemoryStream())
            {
                using (var workbook = new XLWorkbook())
                {
                    var indexWorksheet = workbook.Worksheets.Add("Index");
                    AddIndexSheetContent(indexWorksheet, report, parameters);

                    var dataWorksheet = workbook.Worksheets.Add("Data");
                    AddDataSheetContent(dataWorksheet, columns, dataTable);

                    workbook.SaveAs(memoryStream);
                }

                return memoryStream.ToArray();
            }
        }

        private static void AddIndexSheetContent(IXLWorksheet worksheet, ReportDetailsModel report, IEnumerable<ReportParameterModel>? parameters)
        {
            worksheet.Cell(1, 1).Value = "Report Name";
            worksheet.Cell(1, 2).Value = report.Name ?? string.Empty;
            worksheet.Cell(2, 1).Value = "Report Description";
            worksheet.Cell(2, 2).Value = report.Description ?? "No description provided.";
            worksheet.Cell(3, 1).Value = "Execution Date";
            worksheet.Cell(3, 2).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            if (parameters != null)
            {
                worksheet.Cell(4, 1).Value = "Parameters";
                int rowIndex = 5;
                foreach (var param in parameters)
                {
                    worksheet.Cell(rowIndex, 1).Value = param.Name ?? string.Empty;
                    worksheet.Cell(rowIndex, 2).Value = param.CurrentValue?.ToString() ?? string.Empty;
                    rowIndex++;
                }
            }

            worksheet.Columns().AdjustToContents();
        }

        private static void AddDataSheetContent(IXLWorksheet worksheet, IEnumerable<ReportColumnDefinitionModel> columns, DataTable dataTable)
        {
            var filteredColumns = columns.Where(column => column.Name != RowIdentifierKey).ToList();

            for (int i = 0; i < filteredColumns.Count; i++)
            {
                worksheet.Cell(1, i + 1).Value = filteredColumns[i].Name ?? string.Empty;
                worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.CoolGrey;
            }

            for (int rowIndex = 0; rowIndex < dataTable.Rows.Count; rowIndex++)
            {
                for (int colIndex = 0; colIndex < filteredColumns.Count; colIndex++)
                {
                    var columnName = filteredColumns[colIndex].Name ?? string.Empty;
                    var cellValue = dataTable.Rows[rowIndex][columnName] ?? DBNull.Value;
                    var cell = worksheet.Cell(rowIndex + 2, colIndex + 1);
                    SetCellValue(cell, cellValue, filteredColumns[colIndex].SqlDataType!);
                }
            }

            var tableRange = worksheet.Range(1, 1, dataTable.Rows.Count + 1, filteredColumns.Count);
            var table = tableRange.CreateTable();
            table.Theme = XLTableTheme.TableStyleMedium9;

            worksheet.Columns().AdjustToContents();
        }

        private static void SetCellValue(IXLCell cell, object value, string sqlDataType)
        {
            if (value == null || value == DBNull.Value)
            {
                cell.Value = string.Empty;
                return;
            }

            var convertedValue = ObjectHelpers.ConvertFromSqlValue(value, sqlDataType);

            if (convertedValue != null)
            {
                switch (convertedValue)
                {
                    case int intValue:
                        cell.Value = intValue;
                        break;
                    case long longValue:
                        cell.Value = longValue;
                        break;
                    case float floatValue:
                        cell.Value = floatValue;
                        break;
                    case double doubleValue:
                        cell.Value = doubleValue;
                        break;
                    case decimal decimalValue:
                        cell.Value = decimalValue;
                        break;
                    case bool boolValue:
                        cell.Value = boolValue;
                        break;
                    case DateTime dateTimeValue:
                        cell.Value = dateTimeValue;
                        break;
                    default:
                        cell.Value = convertedValue.ToString();
                        break;
                }
            }
            else
            {
                cell.Value = string.Empty;
            }
        }
    }
}
