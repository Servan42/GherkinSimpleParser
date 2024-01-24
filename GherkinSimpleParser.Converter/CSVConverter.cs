﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GherkinSimpleParser.Converter
{
    public class CSVConverter
    {
        public List<string> ExportAsCSV(string separator, GherkinObject gherkinObj)
        {
            var csv = new List<string>
            {
                "Number;GIVEN;WHEN;THEN"
            };

            if (gherkinObj.Background.Givens.Count > 0)
            {
                string givenBackground = $"GENERAL PREREQUISITES:{separator}{BuildInstructionBatchLine(separator, gherkinObj.Background.Givens)}";
                csv.Add($";{givenBackground};;");
            }

            int testCount = 1;
            foreach (var scenario in gherkinObj.Scenarios)
            {
                csv.Add($"{testCount};{scenario.Name};;");
                string givenCol = BuildInstructionBatchLine(separator, scenario.Givens);
                string whenCol = BuildInstructionBatchLine(separator, scenario.Whens);
                string thenCol = BuildInstructionBatchLine(separator, scenario.Thens);
                csv.Add($";{givenCol};{whenCol};{thenCol}");
                testCount++;
            }

            return csv;
        }

        private string BuildInstructionBatchLine(string separator, List<Instruction> instructions)
        {
            var flattened = new List<string>();
            foreach(var instruction in instructions)
            {
                var sb = new StringBuilder();
                sb.Append(instruction.MainLine);
                if(instruction.DocStrings.Count > 0)
                {
                    sb.Append(" \"").Append(string.Join(' ', instruction.DocStrings)).Append('"');
                }
                if (instruction.DataTable.Count > 0)
                {
                    var flattenedDataTable = new List<string>();
                    foreach (var tableRow in instruction.DataTable)
                    {
                        flattenedDataTable.AddRange(tableRow);
                    }
                    sb.Append(" \"").Append(string.Join(' ', flattenedDataTable)).Append('"');
                }
                flattened.Add(sb.ToString());
            }
            return string.Join(separator, flattened);
        }

        [Obsolete("Deprecated: Will be removed in future versions. Use/modify the ExcelConverter to your needs instead.")]
        public List<string> ExportAsCSVWithExcelFormulaWrap_FR(GherkinObject gherkinObj)
        {
            return ExportAsCSVWithExcelFormulaWrap("\" & CAR(10) & \"", gherkinObj);
        }

        [Obsolete("Deprecated: Will be removed in future versions. Use/modify the ExcelConverter to your needs instead.")]
        public List<string> ExportAsCSVWithExcelFormulaWrap_EN(GherkinObject gherkinObj)
        {
            return ExportAsCSVWithExcelFormulaWrap("\" & CHAR(10) & \"", gherkinObj);
        }

        [Obsolete("Deprecated: Will be removed in future versions. Use/modify the ExcelConverter to your needs instead.")]
        private List<string> ExportAsCSVWithExcelFormulaWrap(string separator, GherkinObject gherkinObj)
        {
            var csv = new List<string>
            {
                "Number;GIVEN;WHEN;THEN"
            };

            if (gherkinObj.Background.Givens.Count > 0)
            {
                string givenBackground = string.Join(separator, 
                    gherkinObj.Background.Givens
                    .Prepend(new Instruction("GENERAL PREREQUISITES:"))
                    .Select(g => g.MainLine.Replace("\"", "\"\"")));
                csv.Add($";=\"{givenBackground}\";;");
            }

            int testCount = 1;
            foreach (var scenario in gherkinObj.Scenarios)
            {
                csv.Add($"{testCount};{scenario.Name};;");
                string givenCol = string.Join(separator, scenario.Givens.Select(g => g.MainLine.Replace("\"", "\"\"")));
                string whenCol = string.Join(separator, scenario.Whens.Select(g => g.MainLine.Replace("\"", "\"\"")));
                string thenCol = string.Join(separator, scenario.Thens.Select(g => g.MainLine.Replace("\"", "\"\"")));
                csv.Add($";=\"{givenCol}\";{whenCol};=\"{thenCol}\"");
                testCount++;
            }

            return csv;
        }
    }
}
