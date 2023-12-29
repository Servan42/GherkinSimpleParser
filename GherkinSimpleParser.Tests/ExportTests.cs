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
        GherkinObject sut;

        /// <summary>
        /// Load a fully filled GherkinObject
        /// </summary>
        [SetUp]
        public void Setup()
        {
            sut = new()
            {
                FeatureName = "feature name",
                Background = new Background
                {
                    Givens = new List<string>
                    {
                        "Prerequisite_0.1",
                        "Prerequisite_0.2"
                    }
                },
                Scenarios = new List<Scenario>
                {
                    new Scenario
                    {
                        Name = "Test Case 1",
                        Givens = new List<string>
                        {
                            "Prerequisite_1.1",
                            "Prerequisite_1.2"
                        },
                        When = "Action_1",
                        Thens = new List<string>
                        {
                            "Result_1.1",
                            "Result_1.2"
                        },
                    },
                    new Scenario
                    {
                        Name = "Test Case 2",
                        Givens = new List<string>
                        {
                            "Prerequisite_2.1",
                            "Prerequisite_2.2"
                        },
                        When = "Action_2",
                        Thens = new List<string>
                        {
                            "Result_2.1",
                            "Result_2.2"
                        }
                    }
                }
            };
        }

        [Test]
        public void Should_export_as_CSV()
        {
            // When
            var csvResult = sut.ExportAsCSV("|");

            // Then
            CollectionAssert.AreEqual(new List<string>()
            {
                "Number,GIVEN,WHEN,THEN",
                ",GENERAL PREREQUISITES:|Prerequisite_0.1|Prerequisite_0.2,,,",
                "1,Test Case 1,,,",
                ",Prerequisite_1.1|Prerequisite_1.2,Action_1,Result_1.1|Result_1.2",
                "2,Test Case 2,,,",
                ",Prerequisite_2.1|Prerequisite_2.2,Action_2,Result_2.1|Result_2.2"
            },
            csvResult);
        }

        [Test]
        public void Should_export_as_CSV_excel_formula_wrap_french()
        {
            // When
            var csvResult = sut.ExportAsCSVWithExcelFormulaWrap_FR();
            var test = string.Join("\r\n", csvResult);

            // Then
            CollectionAssert.AreEqual(new List<string>()
            {
                "Number,GIVEN,WHEN,THEN",
                ",=\"GENERAL PREREQUISITES:\" & CAR(10) & \"Prerequisite_0.1\" & CAR(10) & \"Prerequisite_0.2\",,,",
                "1,Test Case 1,,,",
                ",=\"Prerequisite_1.1\" & CAR(10) & \"Prerequisite_1.2\",Action_1,=\"Result_1.1\" & CAR(10) & \"Result_1.2\"",
                "2,Test Case 2,,,",
                ",=\"Prerequisite_2.1\" & CAR(10) & \"Prerequisite_2.2\",Action_2,=\"Result_2.1\" & CAR(10) & \"Result_2.2\""
            },
            csvResult);
        }

        [Test]
        public void Should_export_as_CSV_excel_formula_wrap_english()
        {
            // When
            var csvResult = sut.ExportAsCSVWithExcelFormulaWrap_EN();
            var test = string.Join("\r\n", csvResult);

            // Then
            CollectionAssert.AreEqual(new List<string>()
            {
                "Number,GIVEN,WHEN,THEN",
                ",=\"GENERAL PREREQUISITES:\" & CHAR(10) & \"Prerequisite_0.1\" & CHAR(10) & \"Prerequisite_0.2\",,,",
                "1,Test Case 1,,,",
                ",=\"Prerequisite_1.1\" & CHAR(10) & \"Prerequisite_1.2\",Action_1,=\"Result_1.1\" & CHAR(10) & \"Result_1.2\"",
                "2,Test Case 2,,,",
                ",=\"Prerequisite_2.1\" & CHAR(10) & \"Prerequisite_2.2\",Action_2,=\"Result_2.1\" & CHAR(10) & \"Result_2.2\""
            },
            csvResult);
        }
    }
}
