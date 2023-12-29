using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GherkinSimpleParser
{
    public class ExcelConversionManager
    {
        public void AppendTestsToExcelFile(string fileName, List<GherkinObject> featuresAndScenarios)
        {
            File.Copy("Blank_do_not_delete.xlsx", $"{fileName}.xlsx", true);

            Thread.Sleep(2000);

            using (SpreadsheetDocument spreadSheet = SpreadsheetDocument.Open($"{fileName}.xlsx", true))
            {
                // Get the SharedStringTablePart. If it does not exist, create a new one.
                SharedStringTablePart shareStringPart;
                if (spreadSheet.WorkbookPart.GetPartsOfType<SharedStringTablePart>().Count() > 0)
                {
                    shareStringPart = spreadSheet.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First();
                }
                else
                {
                    shareStringPart = spreadSheet.WorkbookPart.AddNewPart<SharedStringTablePart>();
                }

                //WorksheetPart summarySheet = CreateEmptySheet(spreadSheet, "Summary");

                foreach (var feature in featuresAndScenarios)
                {
                    string testSheetName = feature.FeatureName;
                    WorksheetPart workSheetPart = CreateEmptySheet(spreadSheet, testSheetName);
                    InsertJiraTestInSheet(spreadSheet, workSheetPart, shareStringPart, feature);
                    workSheetPart.Worksheet.Save();
                }
                spreadSheet.Save();
            }
        }

        private WorksheetPart CreateEmptySheet(SpreadsheetDocument spreadSheet, string sheetName)
        {
            // Add a blank WorksheetPart.
            WorksheetPart newWorksheetPart = spreadSheet.WorkbookPart.AddNewPart<WorksheetPart>();
            newWorksheetPart.Worksheet = new Worksheet(new SheetData());
            newWorksheetPart.Worksheet.Save();

            Sheets sheets = spreadSheet.WorkbookPart.Workbook.GetFirstChild<Sheets>();
            string relationshipId = spreadSheet.WorkbookPart.GetIdOfPart(newWorksheetPart);

            // Get a unique ID for the new worksheet.
            uint sheetId = 1;
            if (sheets.Elements<Sheet>().Count() > 0)
            {
                sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
            }

            // Give the new worksheet a name.
            //string sheetName = "Sheet" + sheetId;

            // Append the new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = sheetName };
            sheets.Append(sheet);
            newWorksheetPart.Worksheet.Save();

            return newWorksheetPart;
        }

        private void InsertJiraTestInSheet(SpreadsheetDocument spreadSheet, WorksheetPart workSheetPart, SharedStringTablePart shareStringPart, GherkinObject featureAndScenarios)
        {
            InsertTextInCell(shareStringPart, workSheetPart, featureAndScenarios.FeatureName, "B", 2);

            InsertTextInCell(shareStringPart, workSheetPart, "Number", "A", 4);
            InsertTextInCell(shareStringPart, workSheetPart, "GIVEN", "B", 4);
            InsertTextInCell(shareStringPart, workSheetPart, "WHEN", "C", 4);
            InsertTextInCell(shareStringPart, workSheetPart, "THEN", "D", 4);
            InsertTextInCell(shareStringPart, workSheetPart, "Comments", "E", 4);
            InsertTextInCell(shareStringPart, workSheetPart, "Status", "F", 4);

            int lineToFillId = 5;
            if (featureAndScenarios.Background.Givens.Count > 0)
            {
                var generalPrerequisite = new StringBuilder();
                generalPrerequisite.AppendLine("GENERAL PREREQUISITES:");
                foreach(var prerequisite in featureAndScenarios.Background.Givens)
                {
                    generalPrerequisite.Append("- ");
                    generalPrerequisite.AppendLine(prerequisite);
                }

                InsertTextInCell(shareStringPart, workSheetPart, generalPrerequisite.ToString(), "B", lineToFillId);
                lineToFillId++;
            }

            int testId = 1;
            foreach(var scenario in featureAndScenarios.Scenarios)
            {
                InsertTextInCell(shareStringPart, workSheetPart, scenario.Name, "B", lineToFillId);
                lineToFillId++;
                
                InsertTextInCell(shareStringPart, workSheetPart, testId.ToString(), "A", lineToFillId);
                string givens = string.Join('\n', scenario.Givens.Select(x => "- " + x));
                InsertTextInCell(shareStringPart, workSheetPart, givens, "B", lineToFillId);
                InsertTextInCell(shareStringPart, workSheetPart, scenario.When, "C", lineToFillId);
                string thens = string.Join('\n', scenario.Thens.Select(x => "- " + x));
                InsertTextInCell(shareStringPart, workSheetPart, thens, "D", lineToFillId);
                
                lineToFillId++;
                testId++;
            }
        }

        private void InsertTextInCell(SharedStringTablePart shareStringPart, WorksheetPart worksheetPart, string text, string columnLetter, int indexLigne)
        {
            // Insert the text into the SharedStringTablePart.
            int index = InsertSharedStringItem(text, shareStringPart);


            // Insert cell A1 into the new worksheet.
            Cell cell = InsertCellInWorksheet(columnLetter, (uint)indexLigne, worksheetPart);

            // Set the value of cell A1.
            cell.CellValue = new CellValue(index.ToString());
            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

            // Save the new worksheet.
            worksheetPart.Worksheet.Save();
        }

        // Given text and a SharedStringTablePart, creates a SharedStringItem with the specified text 
        // and inserts it into the SharedStringTablePart. If the item already exists, returns its index.
        private static int InsertSharedStringItem(string text, SharedStringTablePart shareStringPart)
        {
            // If the part does not contain a SharedStringTable, create one.
            if (shareStringPart.SharedStringTable == null)
            {
                shareStringPart.SharedStringTable = new SharedStringTable();
            }

            int i = 0;

            // Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
            foreach (SharedStringItem item in shareStringPart.SharedStringTable.Elements<SharedStringItem>())
            {
                if (item.InnerText == text)
                {
                    return i;
                }

                i++;
            }

            // The text does not exist in the part. Create the SharedStringItem and return its index.
            shareStringPart.SharedStringTable.AppendChild(new SharedStringItem(new DocumentFormat.OpenXml.Spreadsheet.Text(text)));
            shareStringPart.SharedStringTable.Save();

            return i;
        }

        // Given a column name, a row index, and a WorksheetPart, inserts a cell into the worksheet. 
        // If the cell already exists, returns it. 
        private static Cell InsertCellInWorksheet(string columnName, uint rowIndex, WorksheetPart worksheetPart)
        {
            Worksheet worksheet = worksheetPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();
            string cellReference = columnName + rowIndex;

            // If the worksheet does not contain a row with the specified row index, insert one.
            Row row;
            if (sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).Count() != 0)
            {
                row = sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
            }
            else
            {
                row = new Row() { RowIndex = rowIndex };
                sheetData.Append(row);
            }

            // If there is not a cell with the specified column name, insert one.  
            if (row.Elements<Cell>().Where(c => c.CellReference.Value == columnName + rowIndex).Count() > 0)
            {
                return row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).First();
            }
            else
            {
                // Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
                Cell refCell = null;
                foreach (Cell cell in row.Elements<Cell>())
                {
                    if (cell.CellReference.Value.Length == cellReference.Length)
                    {
                        if (string.Compare(cell.CellReference.Value, cellReference, true) > 0)
                        {
                            refCell = cell;
                            break;
                        }
                    }
                }

                Cell newCell = new Cell() { CellReference = cellReference };
                row.InsertBefore(newCell, refCell);

                worksheet.Save();
                return newCell;
            }
        }
    }
}
