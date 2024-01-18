using GherkinSimpleParser.Converter;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GherkinSimpleParser.Tests
{
    internal class ExportTests
    {
        GherkinObject gherkinObject;

        /// <summary>
        /// Load a fully filled GherkinObject
        /// </summary>
        [SetUp]
        public void Setup()
        {
            gherkinObject = new()
            {
                FeatureName = "feature name",
                Background = new Background
                {
                    Givens = new List<Instruction>
                    {
                        new("Prerequisite_0.1"),
                        new("Prere\"q\"uisite_0.2")
                    }
                },
                Scenarios = new List<Scenario>
                {
                    new Scenario
                    {
                        Name = "Test Case 1",
                        Givens = new List<Instruction>
                        {
                            new("Prerequisite_1.1"),
                            new("Prere\"q\"uisite_1.2")
                        },
                        When = "Action_1",
                        Thens = new List<Instruction>
                        {
                            new("Result_1.1"),
                            new("Resu\"l\"t_1.2")
                        },
                    },
                    new Scenario
                    {
                        Name = "Test Case 2",
                        Givens = new List<Instruction>
                        {
                            new("Prerequisite_2.1"),
                            new("Prere\"q\"uisite_2.2")
                        },
                        When = "Action_2",
                        Thens = new List<Instruction>
                        {
                            new("Result_2.1"),
                            new("Resu\"l\"t_2.2")
                        }
                    }
                }
            };
        }

        [Test]
        public void Should_export_as_CSV()
        {
            // Given
            var converter = new CSVConverter();

            // When
            var csvResult = converter.ExportAsCSV("|", gherkinObject);

            // Then
            CollectionAssert.AreEqual(new List<string>()
            {
                "Number;GIVEN;WHEN;THEN",
                ";GENERAL PREREQUISITES:|Prerequisite_0.1|Prere\"q\"uisite_0.2;;",
                "1;Test Case 1;;",
                ";Prerequisite_1.1|Prere\"q\"uisite_1.2;Action_1;Result_1.1|Resu\"l\"t_1.2",
                "2;Test Case 2;;",
                ";Prerequisite_2.1|Prere\"q\"uisite_2.2;Action_2;Result_2.1|Resu\"l\"t_2.2"
            },
            csvResult);
        }

        [Test]
        public void Should_export_as_CSV_withdocstrings()
        {
            // Given
            var converter = new CSVConverter();
            gherkinObject.Scenarios.First().Givens.First().DocStrings = new List<string>
            {
                "docstrings1",
                "docstrings2",
            };

            // When
            var csvResult = converter.ExportAsCSV("|", gherkinObject);

            // Then
            CollectionAssert.AreEqual(new List<string>()
            {
                "Number;GIVEN;WHEN;THEN",
                ";GENERAL PREREQUISITES:|Prerequisite_0.1|Prere\"q\"uisite_0.2;;",
                "1;Test Case 1;;",
                ";Prerequisite_1.1 \"docstrings1 docstrings2\"|Prere\"q\"uisite_1.2;Action_1;Result_1.1|Resu\"l\"t_1.2",
                "2;Test Case 2;;",
                ";Prerequisite_2.1|Prere\"q\"uisite_2.2;Action_2;Result_2.1|Resu\"l\"t_2.2"
            },
            csvResult);
        }

        [Test]
        public void Should_export_as_CSV_excel_formula_wrap_french()
        {
            // Given
            var converter = new CSVConverter();

            // When
            var csvResult = converter.ExportAsCSVWithExcelFormulaWrap_FR(gherkinObject);
            var test = string.Join("\r\n", csvResult);

            // Then
            CollectionAssert.AreEqual(new List<string>()
            {
                "Number;GIVEN;WHEN;THEN",
                ";=\"GENERAL PREREQUISITES:\" & CAR(10) & \"Prerequisite_0.1\" & CAR(10) & \"Prere\"\"q\"\"uisite_0.2\";;",
                "1;Test Case 1;;",
                ";=\"Prerequisite_1.1\" & CAR(10) & \"Prere\"\"q\"\"uisite_1.2\";Action_1;=\"Result_1.1\" & CAR(10) & \"Resu\"\"l\"\"t_1.2\"",
                "2;Test Case 2;;",
                ";=\"Prerequisite_2.1\" & CAR(10) & \"Prere\"\"q\"\"uisite_2.2\";Action_2;=\"Result_2.1\" & CAR(10) & \"Resu\"\"l\"\"t_2.2\""
            },
            csvResult);
        }

        [Test]
        public void Should_export_as_CSV_excel_formula_wrap_english()
        {
            // Given
            var converter = new CSVConverter();

            // When
            var csvResult = converter.ExportAsCSVWithExcelFormulaWrap_EN(gherkinObject);
            var test = string.Join("\r\n", csvResult);

            // Then
            CollectionAssert.AreEqual(new List<string>()
            {
                "Number;GIVEN;WHEN;THEN",
                ";=\"GENERAL PREREQUISITES:\" & CHAR(10) & \"Prerequisite_0.1\" & CHAR(10) & \"Prere\"\"q\"\"uisite_0.2\";;",
                "1;Test Case 1;;",
                ";=\"Prerequisite_1.1\" & CHAR(10) & \"Prere\"\"q\"\"uisite_1.2\";Action_1;=\"Result_1.1\" & CHAR(10) & \"Resu\"\"l\"\"t_1.2\"",
                "2;Test Case 2;;",
                ";=\"Prerequisite_2.1\" & CHAR(10) & \"Prere\"\"q\"\"uisite_2.2\";Action_2;=\"Result_2.1\" & CHAR(10) & \"Resu\"\"l\"\"t_2.2\""
            },
            csvResult);
        }
    }
}
