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
    }
}
