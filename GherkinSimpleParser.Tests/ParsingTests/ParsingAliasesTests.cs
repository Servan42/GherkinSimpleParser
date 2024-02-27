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
    }
}
