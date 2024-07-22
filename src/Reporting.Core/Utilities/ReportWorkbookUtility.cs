namespace Reporting.Core.Utilities
{
    using System;
    using System.Data;
    using System.IO;
    using System.Linq;

    using DocumentFormat.OpenXml;
    using DocumentFormat.OpenXml.Packaging;
    using DocumentFormat.OpenXml.Spreadsheet;

    using Reporting.Core.Constants;
    using Reporting.Core.Entities;
    using Reporting.Core.Helpers;
    using Reporting.Core.Models;

    public static class ReportWorkbookUtility
    {
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
                using (var spreadsheetDocument = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook))
                {
                    var workbookPart = spreadsheetDocument.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();
                    var sheets = spreadsheetDocument.WorkbookPart!.Workbook.AppendChild(new Sheets());

                    var indexSheetData = CreateSheet(spreadsheetDocument, workbookPart, sheets, "Index", 1);
                    AddIndexSheetContent(indexSheetData, report, parameters);

                    var dataSheetData = CreateSheet(spreadsheetDocument, workbookPart, sheets, "Data", 2);
                    AddDataSheetContent(workbookPart, dataSheetData, columns, dataTable);

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

        private static void AddIndexSheetContent(SheetData sheetData, ReportDetailsModel report, ReportParameterModel[]? parameters)
        {
            AddTextCell(sheetData, "A1", "Report Name");
            AddTextCell(sheetData, "B1", report.Name ?? string.Empty);
            AddTextCell(sheetData, "A2", "Report Description");
            AddTextCell(sheetData, "B2", report.Description ?? "No description provided.");
            AddTextCell(sheetData, "A3", "Execution Date");
            AddTextCell(sheetData, "B3", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            if (parameters != null)
            {
                AddTextCell(sheetData, "A4", "Parameters");
                int rowIndex = 5;
                foreach (var param in parameters)
                {
                    AddTextCell(sheetData, $"A{rowIndex}", param.Name ?? string.Empty);
                    AddTextCell(sheetData, $"B{rowIndex}", param.CurrentValue?.ToString() ?? string.Empty);
                    rowIndex++;
                }
            }
        }

        private static void AddDataSheetContent(WorkbookPart workbookPart, SheetData sheetData, IEnumerable<ReportColumnDefinitionModel> columns, DataTable dataTable)
        {
            var filteredColumns = columns.Where(column => column.Name != GridSettings.RowIdentifierKey).ToList();

            var headerRow = new Row();
            foreach (var column in filteredColumns)
            {
                var cell = new Cell
                {
                    CellValue = new CellValue(column.Name),
                    DataType = CellValues.String,
                    StyleIndex = 1
                };
                headerRow.AppendChild(cell);
            }
            sheetData.AppendChild(headerRow);

            foreach (DataRow dataRow in dataTable.Rows)
            {
                var row = new Row();
                foreach (var column in filteredColumns)
                {
                    var cellValue = dataRow[column.Name] ?? DBNull.Value;
                    var cell = ObjectHelpers.CreateCell(cellValue, column.SqlDataType);
                    row.AppendChild(cell);
                }
                sheetData.AppendChild(row);
            }

            // Create table
            CreateTable(workbookPart, sheetData, filteredColumns, (uint)dataTable.Rows.Count + 1);
        }

        private static void CreateTable(WorkbookPart workbookPart, SheetData sheetData, List<ReportColumnDefinitionModel> columns, uint rowCount)
        {
            var worksheet = sheetData.Parent as Worksheet;
            if (worksheet == null) return;

            var tableDefinitionPart = worksheet.WorksheetPart.AddNewPart<TableDefinitionPart>();
            var table = new Table
            {
                Id = 1,
                Name = "DataTable",
                DisplayName = "DataTable",
                Reference = $"A1:{(char)('A' + columns.Count - 1)}{rowCount}",
                TotalsRowShown = false,
                AutoFilter = new AutoFilter { Reference = $"A1:{(char)('A' + columns.Count - 1)}{rowCount}" }
            };

            var tableColumns = new TableColumns { Count = (uint)columns.Count };
            for (int i = 0; i < columns.Count; i++)
            {
                var tableColumn = new TableColumn
                {
                    Id = (uint)(i + 1),
                    Name = columns[i].Name
                };
                tableColumns.Append(tableColumn);
            }
            table.Append(tableColumns);

            tableDefinitionPart.Table = table;
            tableDefinitionPart.Table.Save();

            worksheet.Save();
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
