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
        public void Should_parse_scenario_alias()
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
                Assert.That(result.Scenarios.Last().Thens.Last().MainLine, Is.EqualTo("result"));
            });
        }
    }
}
