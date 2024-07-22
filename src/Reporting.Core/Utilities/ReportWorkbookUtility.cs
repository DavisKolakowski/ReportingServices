namespace Reporting.Core.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using DocumentFormat.OpenXml.Packaging;
    using DocumentFormat.OpenXml.Spreadsheet;
    using DocumentFormat.OpenXml;
    using Reporting.Core.Entities;
    using Reporting.Core.Helpers;
    using System.ComponentModel;
    using Microsoft.IdentityModel.Tokens;
    using Reporting.Core.Constants;
    using Reporting.Core.Models;

    public static class ReportWorkbookUtility
    {
        public static byte[] CreateExcelReport(ReportDetailsModel report, ReportDataModel model, Dictionary<string, object>? parameters = null)
        {
            var columns = report.ColumnDefinitions.AsEnumerable();

            if (model.Data == null)
            {
                throw new ArgumentNullException(nameof(model.Data), "Data cannot be null when creating an Excel report.");
            }

            var data = model.Data.Select(row => row).ToList();

            using (var memoryStream = new MemoryStream())
            {
                using (var spreadsheetDocument = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook))
                {
                    var workbookPart = spreadsheetDocument.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();
                    var sheets = spreadsheetDocument.WorkbookPart!.Workbook.AppendChild(new Sheets());

                    var indexSheetData = CreateSheet(spreadsheetDocument, workbookPart, sheets, "Index", 1);
                    AddIndexSheetContent(indexSheetData, report, parameters);

                    var dataSheetData = CreateSheet(spreadsheetDocument, workbookPart, sheets, "Data", 2);
                    AddDataSheetContent(dataSheetData, columns, data);

                    workbookPart.Workbook.Save();
                }

                return memoryStream.ToArray();
            }
        }

        private static SheetData CreateSheet(SpreadsheetDocument document, WorkbookPart workbookPart, Sheets sheets, string sheetName, uint sheetId)
        {
            var sheetPart = workbookPart.AddNewPart<WorksheetPart>();
            var sheetData = new SheetData();
            sheetPart.Worksheet = new Worksheet(sheetData);

            var sheet = new Sheet()
            {
                Id = document.WorkbookPart?.GetIdOfPart(sheetPart),
                SheetId = sheetId,
                Name = sheetName
            };
            sheets.Append(sheet);

            return sheetData;
        }

        private static void AddIndexSheetContent(SheetData sheetData, ReportDetailsModel report, Dictionary<string, object>? parameters)
        {
            var reportName = report.Name;
            if (string.IsNullOrEmpty(reportName))
            {
                throw new ArgumentNullException(nameof(reportName), "Report name cannot be null or empty.");
            }
            var reportDescription = report.Description;
            if (string.IsNullOrEmpty(reportDescription))
            {
                reportDescription = "No description provided.";
            }
            AddTextCell(sheetData, "A1", "Report Name");
            AddTextCell(sheetData, "B1", reportName);
            AddTextCell(sheetData, "A2", "Report Description");
            AddTextCell(sheetData, "B2", reportDescription);
            AddTextCell(sheetData, "A3", "Execution Date");
            AddTextCell(sheetData, "B3", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            if (parameters != null)
            {
                AddTextCell(sheetData, "A4", "Parameters");
                int rowIndex = 5;
                foreach (var param in parameters)
                {
                    AddTextCell(sheetData, $"A{rowIndex}", param.Key);
                    AddTextCell(sheetData, $"B{rowIndex}", param.Value?.ToString() ?? string.Empty);
                    rowIndex++;
                }
            }
        }

        private static void AddDataSheetContent(SheetData sheetData, IEnumerable<ReportColumnDefinitionModel> columns, IEnumerable<Dictionary<string, object>> data)
        {
            if (columns == null)
            {
                throw new ArgumentNullException(nameof(columns), "Columns cannot be null when creating an Excel report.");
            }

            var filteredColumns = columns.Where(column => column.Name != GridSettings.RowIdentifierKey).ToList();

            var headerRow = new Row();
            foreach (var column in filteredColumns)
            {
                if (string.IsNullOrEmpty(column.Name))
                {
                    throw new ArgumentNullException(nameof(column.Name), "Column name cannot be null or empty.");
                }

                var cell = new Cell
                {
                    CellValue = new CellValue(column.Name),
                    DataType = CellValues.String
                };

                headerRow.AppendChild(cell);
            }
            sheetData.AppendChild(headerRow);

            foreach (var row in data)
            {
                var dataRow = new Row();
                foreach (var column in filteredColumns)
                {
                    if (string.IsNullOrEmpty(column.Name))
                    {
                        throw new ArgumentNullException(nameof(column.Name), "Column name cannot be null or empty.");
                    }
                    if (string.IsNullOrEmpty(column.SqlDataType))
                    {
                        throw new ArgumentNullException(nameof(column.SqlDataType), "Column SQL data type cannot be null or empty.");
                    }
                    var cellValue = row.TryGetValue(column.Name, out var value) ? value : null;
                    var cell = ObjectHelpers.CreateCell(cellValue, column.SqlDataType);
                    dataRow.AppendChild(cell);
                }
                sheetData.AppendChild(dataRow);
            }
        }

        private static void AddTextCell(SheetData sheetData, string cellReference, string text)
        {
            var rowIndex = GetRowIndex(cellReference);
            var row = sheetData.Elements<Row>().FirstOrDefault(r => r.RowIndex == rowIndex);
            if (row == null)
            {
                row = new Row() { RowIndex = rowIndex };
                sheetData.Append(row);
            }

            var cell = new Cell()
            {
                CellReference = cellReference,
                CellValue = new CellValue(text ?? string.Empty),
                DataType = CellValues.String
            };
            row.Append(cell);
        }

        private static uint GetRowIndex(string cellReference)
        {
            string rowPart = new string(cellReference.Where(char.IsDigit).ToArray());
            return uint.TryParse(rowPart, out uint rowIndex) ? rowIndex : 0;
        }
    }
}
