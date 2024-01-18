using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GherkinSimpleParser.Tests.ParsingTests
{
    internal class ParsingDocStringsTests
    {
        [Test]
        public void Should_parse_doc_strings()
        {
            // Given
            var inputLines = new List<string>
            {
                "   Background:",
                "       Given prerequisite",
                "       \"\"\"",
                "       docstring_bg_given_1",
                "       docstring_bg_given_2",
                "       \"\"\"",
                "       And prerequisite1",
                "       \"\"\"",
                "       docstring_bg_given_and_1",
                "       docstring_bg_given_and_2",
                "       \"\"\"",
                "   Scenario: Should do something",
                "       Given prerequisite2",
                "       \"\"\"",
                "       docstring_sc_given_1",
                "       docstring_sc_given_2",
                "       \"\"\"",
                "       And prerequisite3",
                "       \"\"\"",
                "       docstring_sc_given_and_1",
                "       docstring_sc_given_and_2",
                "       \"\"\"",
                "       When action",
                "       Then result",
                "       \"\"\"",
                "       docstring_sc_then_1",
                "       docstring_sc_then_2",
                "       \"\"\"",
                "       And result1",
                "       \"\"\"",
                "       docstring_sc_then_and_1",
                "       docstring_sc_then_and_2",
                "       \"\"\"",
            };

            // When
            var result = new GherkinObjectParser(inputLines).Parse();

            // Then
            Assert.Multiple(() =>
            {
                CollectionAssert.AreEqual(
                    new List<string> { "docstring_bg_given_1", "docstring_bg_given_2" },
                    result.Background.Givens.First().DocStrings);
                CollectionAssert.AreEqual(
                    new List<string> { "docstring_bg_given_and_1", "docstring_bg_given_and_2" },
                    result.Background.Givens.Last().DocStrings);

                CollectionAssert.AreEqual(
                    new List<string> { "docstring_sc_given_1", "docstring_sc_given_2" },
                    result.Scenarios.First().Givens.First().DocStrings);
                CollectionAssert.AreEqual(
                    new List<string> { "docstring_sc_given_and_1", "docstring_sc_given_and_2" },
                    result.Scenarios.First().Givens.Last().DocStrings);

                CollectionAssert.AreEqual(
                    new List<string> { "docstring_sc_then_1", "docstring_sc_then_2" },
                    result.Scenarios.First().Thens.First().DocStrings);
                CollectionAssert.AreEqual(
                    new List<string> { "docstring_sc_then_and_1", "docstring_sc_then_and_2" },
                    result.Scenarios.First().Thens.Last().DocStrings);
            });
        }

        [Test]
        public void Should_parse_doc_strings_and_indent()
        {
            // Given
            var inputLines = new List<string>
            {
                "    Scenario: Should do something",
                "        Given prerequisite2",
                "\"\"\"",
                "docstring_sc_given_1",
                "    docstring_sc_given_2",
                "\"\"\"",
                "        And prerequisite3",
                "        \"\"\"",
                "        docstring_sc_given_and_1",
                "            docstring_sc_given_and_2",
                "        \"\"\"",
                "        When action",
                "        Then result",
                "        \"\"\"",
                "        docstring_sc_then_1",
                "docstring_sc_then_2",
                "        \"\"\"",
            };

            // When
            var result = new GherkinObjectParser(inputLines).Parse();

            // Then
            Assert.Multiple(() =>
            {
                CollectionAssert.AreEqual(
                    new List<string> { "docstring_sc_given_1", "    docstring_sc_given_2" },
                    result.Scenarios.First().Givens.First().DocStrings);
                CollectionAssert.AreEqual(
                    new List<string> { "docstring_sc_given_and_1", "    docstring_sc_given_and_2" },
                    result.Scenarios.First().Givens.Last().DocStrings);
                CollectionAssert.AreEqual(
                    new List<string> { "docstring_sc_then_1", "docstring_sc_then_2" },
                    result.Scenarios.First().Thens.Last().DocStrings);
            });
        }
    }
}
