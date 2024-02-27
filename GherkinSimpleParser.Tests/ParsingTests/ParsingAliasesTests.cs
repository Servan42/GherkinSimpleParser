using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GherkinSimpleParser.Tests.ParsingTests
{
    internal class ParsingAliasesTests
    {
        [Test]
        public void Should_parse_Scenario_alias()
        {
            // Given
            var inputLines = new List<string>
            {
                "    Example: Should do something",
                "       Given prerequisite",
                "       When action",
                "       Then result",
            };

            // When
            var result = new GherkinObjectParser(inputLines).Parse();

            // Then
            Assert.Multiple(() =>
            {
                Assert.That(result.Scenarios.First().Givens.First().MainLine, Is.EqualTo("prerequisite"));
                Assert.That(result.Scenarios.First().Whens.First().MainLine, Is.EqualTo("action"));
                Assert.That(result.Scenarios.First().Thens.First().MainLine, Is.EqualTo("result"));
            });
        }

        [Test]
        public void Should_parse_And_alias()
        {
            // Given
            var inputLines = new List<string>
            {
                "    Scenario: Should do something",
                "       Given prerequisite",
                "       * prerequisite2",
                "       When action",
                "       * action2",
                "       Then result",
                "       * result2",
            };

            // When
            var result = new GherkinObjectParser(inputLines).Parse();

            // Then
            Assert.Multiple(() =>
            {
                Assert.That(result.Scenarios.First().Givens.First().MainLine, Is.EqualTo("prerequisite"));
                Assert.That(result.Scenarios.First().Givens.Last().MainLine, Is.EqualTo("prerequisite2"));
                Assert.That(result.Scenarios.First().Whens.First().MainLine, Is.EqualTo("action"));
                Assert.That(result.Scenarios.First().Whens.Last().MainLine, Is.EqualTo("action2"));
                Assert.That(result.Scenarios.First().Thens.First().MainLine, Is.EqualTo("result"));
                Assert.That(result.Scenarios.First().Thens.Last().MainLine, Is.EqualTo("result2"));
            });
        }

        [Test]
        public void Should_parse_ScenarioOutline_aliases()
        {
            // Given
            var inputLines = new List<string>
            {
                "Scenario Template: eating",
                "Given there are<start> cucumbers",
                "When I eat <eat> cucumbers",
                "Then I should have <left> cucumbers",
                "",
                "Scenarios:",
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
    }
}
