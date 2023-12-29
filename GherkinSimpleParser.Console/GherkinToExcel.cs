using DocumentFormat.OpenXml.Packaging;
using ExcelConversionManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GherkinSimpleParser.Console
{
    internal class GherkinToExcel : ExcelConversionManagerService<GherkinObject>
    {
        protected override void AppendDataToExcelFile(List<GherkinObject> featuresAndScenarios)
        {
            foreach (var feature in featuresAndScenarios)
            {
                string testSheetName = feature.FeatureName;
                WorksheetPart workSheetPart = CreateEmptySheet(testSheetName);
                InsertSheetHeader(workSheetPart, feature);
                InsertGherkinObjectInSheet(workSheetPart, feature);
                workSheetPart.Worksheet.Save();
            }
        }

        private void InsertSheetHeader(WorksheetPart workSheetPart, GherkinObject featureAndScenarios)
        {
            InsertTextInCell(workSheetPart, featureAndScenarios.FeatureName, "B", 2);

            InsertTextInCell(workSheetPart, "Number", "A", 4);
            InsertTextInCell(workSheetPart, "GIVEN", "B", 4);
            InsertTextInCell(workSheetPart, "WHEN", "C", 4);
            InsertTextInCell(workSheetPart, "THEN", "D", 4);
            InsertTextInCell(workSheetPart, "Comments", "E", 4);
            InsertTextInCell(workSheetPart, "Status", "F", 4);
        }

        private void InsertGherkinObjectInSheet(WorksheetPart workSheetPart, GherkinObject featureAndScenarios)
        {
            int lineToFillId = 5;
            if (featureAndScenarios.Background.Givens.Count > 0)
            {
                var generalPrerequisite = new StringBuilder();
                generalPrerequisite.AppendLine("GENERAL PREREQUISITES:");
                foreach (var prerequisite in featureAndScenarios.Background.Givens)
                {
                    generalPrerequisite.Append("- ");
                    generalPrerequisite.AppendLine(prerequisite);
                }

                InsertTextInCell(workSheetPart, generalPrerequisite.ToString(), "B", lineToFillId);
                lineToFillId++;
            }

            int testId = 1;
            foreach (var scenario in featureAndScenarios.Scenarios)
            {
                InsertTextInCell(workSheetPart, scenario.Name, "B", lineToFillId);
                lineToFillId++;

                InsertTextInCell(workSheetPart, testId.ToString(), "A", lineToFillId);
                string givens = string.Join('\n', scenario.Givens.Select(x => "- " + x));
                InsertTextInCell(workSheetPart, givens, "B", lineToFillId);
                InsertTextInCell(workSheetPart, scenario.When, "C", lineToFillId);
                string thens = string.Join('\n', scenario.Thens.Select(x => "- " + x));
                InsertTextInCell(workSheetPart, thens, "D", lineToFillId);

                lineToFillId++;
                testId++;
            }
        }
    }
}
