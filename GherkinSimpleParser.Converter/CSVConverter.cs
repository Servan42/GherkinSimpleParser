using System;
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
                string givenBackground = string
                    .Join(separator, 
                        gherkinObj.Background.Givens
                        .Prepend(new Instruction("GENERAL PREREQUISITES:"))
                        .Select(i => i.MainLine));
                csv.Add($";{givenBackground};;");
            }

            int testCount = 1;
            foreach (var scenario in gherkinObj.Scenarios)
            {
                csv.Add($"{testCount};{scenario.Name};;");
                string givenCol = string.Join(separator, scenario.Givens.Select(x => x.MainLine));
                string whenCol = scenario.When;
                string thenCol = string.Join(separator, scenario.Thens.Select(x => x.MainLine));
                csv.Add($";{givenCol};{whenCol};{thenCol}");
                testCount++;
            }

            return csv;
        }

        public List<string> ExportAsCSVWithExcelFormulaWrap_FR(GherkinObject gherkinObj)
        {
            return ExportAsCSVWithExcelFormulaWrap("\" & CAR(10) & \"", gherkinObj);
        }

        public List<string> ExportAsCSVWithExcelFormulaWrap_EN(GherkinObject gherkinObj)
        {
            return ExportAsCSVWithExcelFormulaWrap("\" & CHAR(10) & \"", gherkinObj);
        }

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
                string whenCol = scenario.When;
                string thenCol = string.Join(separator, scenario.Thens.Select(g => g.MainLine.Replace("\"", "\"\"")));
                csv.Add($";=\"{givenCol}\";{whenCol};=\"{thenCol}\"");
                testCount++;
            }

            return csv;
        }
    }
}
