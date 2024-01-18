using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GherkinSimpleParser.Converter
{
    public class ExcelConverter
    {
        public void AppendDataToExcelFile(string outputFilePath, List<GherkinObject> gherkinObj)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var stream = new MemoryStream();
            using (var excelPack = new ExcelPackage(stream))
            {
                AppendSheetsToExcelFile(gherkinObj, excelPack);
                excelPack.SaveAs(stream);
            }

            stream.Position = 0;
            File.WriteAllBytes(outputFilePath, stream.ToArray());
        }

        private void AppendSheetsToExcelFile(List<GherkinObject> featuresAndScenarios, ExcelPackage excelPack)
        {
            foreach (var feature in featuresAndScenarios)
            {
                string testSheetName = feature.FeatureName;
                var ws = excelPack.Workbook.Worksheets.Add(testSheetName);
                ws.Column(2).Width = 50;
                ws.Column(3).Width = 50;
                ws.Column(4).Width = 50;
                ws.Column(5).Width = 50;
                InsertSheetHeader(feature, ws);
                InsertGherkinObjectInSheet(feature, ws);
            }
        }

        private void InsertSheetHeader(GherkinObject featureAndScenarios, ExcelWorksheet ws)
        {
            ws.Cells["B2"].Value = featureAndScenarios.FeatureName;
            ws.Cells["B2:E2"].Merge = true;
            ws.Cells["B2:E2"].Style.Font.Bold = true;
            ws.Cells["B2:E2"].Style.Font.Size = 20;
            ws.AlignCenter("B2:E2", true, true);

            ws.Cells["A4:F4"].Style.Font.Bold = true;
            ws.Cells["A4:F4"].Style.Font.Size = 12;
            ws.AlignCenter("A4:F4", true, true);
            ws.SetColor("A4:F4", Color.FromArgb(255, 68, 114, 196));
            ws.Cells["A4"].Value = "Number";
            ws.Cells["B4"].Value = "GIVEN";
            ws.Cells["C4"].Value = "WHEN";
            ws.Cells["D4"].Value = "THEN";
            ws.Cells["E4"].Value = "Comments";
            ws.Cells["F4"].Value = "Status";
        }

        private void InsertGherkinObjectInSheet(GherkinObject featureAndScenarios, ExcelWorksheet ws)
        {
            int lineToFillId = 5;
            if (featureAndScenarios.Background.Givens.Count > 0)
            {
                var generalPrerequisite = new StringBuilder();
                generalPrerequisite.AppendLine("GENERAL PREREQUISITES:");
                generalPrerequisite.AppendLine(BuildInstructionBatch(featureAndScenarios.Background.Givens));

                ws.Cells[$"B{lineToFillId}"].Value = generalPrerequisite.ToString();
                ws.AlignCenter($"B{lineToFillId}", true, false);
                ws.SetColor($"A{lineToFillId}:F{lineToFillId}", Color.FromArgb(255, 180, 198, 231));
                lineToFillId++;
            }

            int testId = 1;
            foreach (var scenario in featureAndScenarios.Scenarios)
            {
                ws.Cells[$"A{lineToFillId}"].Value = testId;
                ws.AlignCenter($"A{lineToFillId}:F{lineToFillId}", true, false);
                ws.AlignCenter($"A{lineToFillId}", false, true);

                ws.Cells[$"B{lineToFillId}"].Value = scenario.Name;
                ws.SetColor($"A{lineToFillId}:F{lineToFillId}", Color.FromArgb(255, 180, 198, 231));
                lineToFillId++;

                ws.Cells[$"B{lineToFillId}"].Value = BuildInstructionBatch(scenario.Givens);
                ws.Cells[$"C{lineToFillId}"].Value = scenario.When;
                ws.Cells[$"D{lineToFillId}"].Value = BuildInstructionBatch(scenario.Thens);
                ws.AlignCenter($"A{lineToFillId}:F{lineToFillId}", true, false);

                lineToFillId++;
                testId++;
            }
        }

        private string BuildInstructionBatch(List<Instruction> instructions)
        {
            StringBuilder sb = new();
            foreach (var instruction in instructions)
            {
                sb.Append("- ").AppendLine(instruction.MainLine);
                if (instruction.DocStrings.Count > 0)
                {
                    sb.AppendLine("\"");
                    foreach (var docstring in instruction.DocStrings)
                        sb.AppendLine(docstring);
                    sb.AppendLine("\"");
                }
            }
            return RemoveLastNewLine(sb.ToString());
        }

        private string RemoveLastNewLine(string s)
        {
            if (s.Length < 2)
                return s;
            return s.Substring(0, s.Length - 2);
        }
    }
}
