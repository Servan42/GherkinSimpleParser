using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GherkinSimpleParser.Tests.ParsingTests
{
    internal class ParsingScenarioOutlineTests
    {
        [Test]
        public void Should_parse_scenario_outline_as_classic_scenario()
        {
            // Given
            var inputLines = new List<string>
            {
                "Scenario Outline: eating",
                "Given there are<start> cucumbers",
                "When I eat <eat> cucumbers",
                "Then I should have <left> cucumbers",
                "",
                "Examples:",
                "   | start | eat | left |",
                "   | 12 | 5 | 7 |",
                "   | 20 | 5 | 15 |",
            };

            // When
            var result = new GherkinObjectParser(inputLines).Parse();

            // Then
            Assert.Multiple(() =>
            {
                Assert.That(result.Scenarios[0].Name, Is.EqualTo("eating"));
                Assert.That(result.Scenarios[0].Givens[0].MainLine, Is.EqualTo("there are<start> cucumbers"));
                Assert.That(result.Scenarios[0].Whens[0].MainLine, Is.EqualTo("I eat <eat> cucumbers"));
                Assert.That(result.Scenarios[0].Thens[0].MainLine, Is.EqualTo("I should have <left> cucumbers"));
                Assert.That(result.Scenarios[0].IsScenarioOutline, Is.EqualTo(true));
                CollectionAssert.AreEqual(new Dictionary<string, List<string>>
                {
                    { "start", new List<string> { "12", "20" } },
                    { "eat", new List<string> { "5", "5" } },
                    { "left", new List<string> { "7", "15" } },
                },
                result.Scenarios[0].Examples);
            });
        }

        [Test]
        public void Should_parse_scenario_outline_correctly_amoung_other_scenarios()
        {
            // Given
            var inputLines = new List<string>
            {
                "Scenario: before",
                "",
                "Scenario Outline: eating",
                "Given there are<start> cucumbers",
                "When I eat <eat> cucumbers",
                "Then I should have <left> cucumbers",
                "",
                "Examples:",
                "   | start | eat | left |",
                "   | 12 | 5 | 7 |",
                "   | 20 | 5 | 15 |",
                "",
                "Scenario: after",
            };

            // When
            var result = new GherkinObjectParser(inputLines).Parse();

            // Then
            Assert.Multiple(() =>
            {
                Assert.That(result.Scenarios[0].Name, Is.EqualTo("before"));
                Assert.That(result.Scenarios[1].Name, Is.EqualTo("eating"));
                Assert.That(result.Scenarios[2].Name, Is.EqualTo("after"));
            });
        }

        [Test]
        public void Should_throw_exception_when_encountering_Examples_without_a_scenario_outline()
        {
            // Given
            var inputLines = new List<string>
            {
                "Scenario: eating",
                "Given there are<start> cucumbers",
                "When I eat <eat> cucumbers",
                "Then I should have <left> cucumbers",
                "",
                "Examples:",
                "   | start | eat | left |",
                "   | 12 | 5 | 7 |",
                "   | 20 | 5 | 15 |",
            };

            // When/Then
            Assert.Throws<GherkinObjectParser.StateMachineException>(() => new GherkinObjectParser(inputLines).Parse());
        }

        [Test]
        public void Should_throw_exception_when_encountering_a_scenario_outline_without_examples()
        {
            // Given
            var inputLines = new List<string>
            {
                "Scenario Outline: eating",
                "Given there are<start> cucumbers",
                "When I eat <eat> cucumbers",
                "Then I should have <left> cucumbers",
            };

            // When/Then
            var ex = Assert.Throws<ArgumentException>(() => new GherkinObjectParser(inputLines).Parse());
            Assert.That(ex.Message, Is.EqualTo("At least one Scenario Outline does not have Examples. (Parameter 'inputLines')"));
        }

        [Test]
        public void Should_throw_exception_when_encountering_a_scenario_outline_without_examples_data()
        {
            // Given
            var inputLines = new List<string>
            {
                "Scenario Outline: eating",
                "Given there are<start> cucumbers",
                "When I eat <eat> cucumbers",
                "Then I should have <left> cucumbers",
                "",
                "Examples:",
                "   | start | eat | left |",
            };

            // When/Then
            var ex = Assert.Throws<ArgumentException>(() => new GherkinObjectParser(inputLines).Parse());
            Assert.That(ex.Message, Is.EqualTo("At least one Scenario Outline does not have Examples data. (Parameter 'inputLines')"));
        }

        [Test]
        public void Should_throw_exception_when_Examples_have_duplicate_headers()
        {
            // Given
            var inputLines = new List<string>
            {
                "Scenario Outline: eating",
                "Given there are<start> cucumbers",
                "When I eat <eat> cucumbers",
                "Then I should have <left> cucumbers",
                "",
                "Examples:",
                "   | start | start | left |",
                "   | 12 | 5 | 7 |",
                "   | 20 | 5 | 15 |",
            };

            // When/Then
            var ex = Assert.Throws<ArgumentException>(() => new GherkinObjectParser(inputLines).Parse());
            Assert.That(ex.Message, Is.EqualTo("Line 7: An Example cannot have duplicate headers (Parameter 'inputLines')"));
        }

        [Test]
        public void Should_convert_scenario_outline_to_regular_scenario()
        {
            // Given
            var inputLines = new List<string>
            {
                "Scenario: before",
                "",
                "Scenario Outline: eating",
                "Given there are <start><eat> cucumbers",
                "And there are <start> cucumbers2",
                "When I eat <eat> cucumbers",
                "And I eat <eat> cucumbers2",
                "Then I should have <left> cucumbers",
                "And I should have <left> cucumbers2",
                "",
                "Examples:",
                "   | start | eat | left |",
                "   | 12 | 5 | 7 |",
                "   | 20 | 6 | 15 |",
                "",
                "Scenario: after"
            };

            var result = new GherkinObjectParser(inputLines).Parse();
            
            // When
            result.TransformScenarioOutlineToClassicScenarioAndOverrideScenarioList();

            // Then
            Assert.Multiple(() =>
            {
                Assert.That(result.Scenarios[0].Name, Is.EqualTo("before"));

                Assert.That(result.Scenarios[1].Name, Is.EqualTo("eating Example 1"));
                Assert.That(result.Scenarios[1].Givens[0].MainLine, Is.EqualTo("there are 125 cucumbers"));
                Assert.That(result.Scenarios[1].Givens[1].MainLine, Is.EqualTo("there are 12 cucumbers2"));
                Assert.That(result.Scenarios[1].Whens[0].MainLine, Is.EqualTo("I eat 5 cucumbers"));
                Assert.That(result.Scenarios[1].Whens[1].MainLine, Is.EqualTo("I eat 5 cucumbers2"));
                Assert.That(result.Scenarios[1].Thens[0].MainLine, Is.EqualTo("I should have 7 cucumbers"));
                Assert.That(result.Scenarios[1].Thens[1].MainLine, Is.EqualTo("I should have 7 cucumbers2"));
                Assert.That(result.Scenarios[1].IsScenarioOutline, Is.EqualTo(false));
                Assert.That(result.Scenarios[1].Examples.Count, Is.EqualTo(0));

                Assert.That(result.Scenarios[2].Name, Is.EqualTo("eating Example 2"));
                Assert.That(result.Scenarios[2].Givens[0].MainLine, Is.EqualTo("there are 206 cucumbers"));
                Assert.That(result.Scenarios[2].Givens[1].MainLine, Is.EqualTo("there are 20 cucumbers2"));
                Assert.That(result.Scenarios[2].Whens[0].MainLine, Is.EqualTo("I eat 6 cucumbers"));
                Assert.That(result.Scenarios[2].Whens[1].MainLine, Is.EqualTo("I eat 6 cucumbers2"));
                Assert.That(result.Scenarios[2].Thens[0].MainLine, Is.EqualTo("I should have 15 cucumbers"));
                Assert.That(result.Scenarios[2].Thens[1].MainLine, Is.EqualTo("I should have 15 cucumbers2"));
                Assert.That(result.Scenarios[2].IsScenarioOutline, Is.EqualTo(false));
                Assert.That(result.Scenarios[2].Examples.Count, Is.EqualTo(0));

                Assert.That(result.Scenarios[3].Name, Is.EqualTo("after"));
            });
        }

        [Test]
        public void Should_convert_scenario_outline_to_regular_scenario_for_datatables()
        {
            // Given
            var inputLines = new List<string>
            {
                "Scenario Outline: eating",
                "Given there are <start> cucumbers",
                "| this | is | a <start><eat> datatable |",
                "| this | is | a <start> datatable2 |",
                "And there are <start> cucumbers2",
                "| this | is | a <start> datatable3 |",
                "| this | is | a <start> datatable4 |",
                "When I eat <eat> cucumbers",
                "| this <eat> | is | a datatable |",
                "| this2 <eat> | is | a datatable |",
                "Then I should have <left> cucumbers",
                "| this | <left>is | a datatable |",
                "| this | <left>is2 | a datatable |",
                "",
                "Examples:",
                "   | start | eat | left |",
                "   | 12 | 5 | 7 |",
                "   | 20 | 6 | 15 |",
            };

            var result = new GherkinObjectParser(inputLines).Parse();

            // When
            result.TransformScenarioOutlineToClassicScenarioAndOverrideScenarioList();

            // Then
            Assert.Multiple(() =>
            {
                Assert.That(result.Scenarios[0].Givens[0].DataTable[0][2], Is.EqualTo("a 125 datatable"));
                Assert.That(result.Scenarios[0].Givens[0].DataTable[1][2], Is.EqualTo("a 12 datatable2"));
                Assert.That(result.Scenarios[0].Givens[1].DataTable[0][2], Is.EqualTo("a 12 datatable3"));
                Assert.That(result.Scenarios[0].Givens[1].DataTable[1][2], Is.EqualTo("a 12 datatable4"));
                Assert.That(result.Scenarios[0].Whens[0].DataTable[0][0], Is.EqualTo("this 5"));
                Assert.That(result.Scenarios[0].Whens[0].DataTable[1][0], Is.EqualTo("this2 5"));
                Assert.That(result.Scenarios[0].Thens[0].DataTable[0][1], Is.EqualTo("7is"));
                Assert.That(result.Scenarios[0].Thens[0].DataTable[1][1], Is.EqualTo("7is2"));

                Assert.That(result.Scenarios[1].Givens[0].DataTable[0][2], Is.EqualTo("a 206 datatable"));
                Assert.That(result.Scenarios[1].Givens[0].DataTable[1][2], Is.EqualTo("a 20 datatable2"));
                Assert.That(result.Scenarios[1].Givens[1].DataTable[0][2], Is.EqualTo("a 20 datatable3"));
                Assert.That(result.Scenarios[1].Givens[1].DataTable[1][2], Is.EqualTo("a 20 datatable4"));
                Assert.That(result.Scenarios[1].Whens[0].DataTable[0][0], Is.EqualTo("this 6"));
                Assert.That(result.Scenarios[1].Whens[0].DataTable[1][0], Is.EqualTo("this2 6"));
                Assert.That(result.Scenarios[1].Thens[0].DataTable[0][1], Is.EqualTo("15is"));
                Assert.That(result.Scenarios[1].Thens[0].DataTable[1][1], Is.EqualTo("15is2"));
            });
        }

        [Test]
        public void Should_convert_scenario_outline_to_regular_scenario_for_docstrings()
        {
            // Given
            var inputLines = new List<string>
            {
                "Scenario Outline: eating",
                "Given there are <start> cucumbers",
                "\"\"\"",
                "this is a <start><eat> docstring",
                "this is a <start> docstring2",
                "\"\"\"",
                "And there are <start> cucumbers2",
                "\"\"\"",
                "this is a <start> docstring3",
                "this is a <start> docstring4",
                "\"\"\"",
                "When I eat <eat> cucumbers",
                "\"\"\"",
                "this <eat> is a docstring",
                "this2 <eat> is a docstring",
                "\"\"\"",
                "Then I should have <left> cucumbers",
                "\"\"\"",
                "this <left>is a docstring",
                "this <left>is2 a docstring",
                "\"\"\"",
                "",
                "Examples:",
                "   | start | eat | left |",
                "   | 12 | 5 | 7 |",
                "   | 20 | 6 | 15 |",
            };

            var result = new GherkinObjectParser(inputLines).Parse();

            // When
            result.TransformScenarioOutlineToClassicScenarioAndOverrideScenarioList();

            // Then
            Assert.Multiple(() =>
            {
                Assert.That(result.Scenarios[0].Givens[0].DocStrings[0], Is.EqualTo("this is a 125 docstring"));
                Assert.That(result.Scenarios[0].Givens[0].DocStrings[1], Is.EqualTo("this is a 12 docstring2"));
                Assert.That(result.Scenarios[0].Givens[1].DocStrings[0], Is.EqualTo("this is a 12 docstring3"));
                Assert.That(result.Scenarios[0].Givens[1].DocStrings[1], Is.EqualTo("this is a 12 docstring4"));
                Assert.That(result.Scenarios[0].Whens[0].DocStrings[0], Is.EqualTo("this 5 is a docstring"));
                Assert.That(result.Scenarios[0].Whens[0].DocStrings[1], Is.EqualTo("this2 5 is a docstring"));
                Assert.That(result.Scenarios[0].Thens[0].DocStrings[0], Is.EqualTo("this 7is a docstring"));
                Assert.That(result.Scenarios[0].Thens[0].DocStrings[1], Is.EqualTo("this 7is2 a docstring"));

                Assert.That(result.Scenarios[1].Givens[0].DocStrings[0], Is.EqualTo("this is a 206 docstring"));
                Assert.That(result.Scenarios[1].Givens[0].DocStrings[1], Is.EqualTo("this is a 20 docstring2"));
                Assert.That(result.Scenarios[1].Givens[1].DocStrings[0], Is.EqualTo("this is a 20 docstring3"));
                Assert.That(result.Scenarios[1].Givens[1].DocStrings[1], Is.EqualTo("this is a 20 docstring4"));
                Assert.That(result.Scenarios[1].Whens[0].DocStrings[0], Is.EqualTo("this 6 is a docstring"));
                Assert.That(result.Scenarios[1].Whens[0].DocStrings[1], Is.EqualTo("this2 6 is a docstring"));
                Assert.That(result.Scenarios[1].Thens[0].DocStrings[0], Is.EqualTo("this 15is a docstring"));
                Assert.That(result.Scenarios[1].Thens[0].DocStrings[1], Is.EqualTo("this 15is2 a docstring"));
            });
        }
    }
}
