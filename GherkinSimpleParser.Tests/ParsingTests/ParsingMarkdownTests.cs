using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GherkinSimpleParser.Tests.ParsingTests
{
    internal class ParsingMarkdownTests
    {
        private List<string> completeInputLines;

        [SetUp]
        public void Setup()
        {
            completeInputLines = new List<string>
            {
                "Feature: myFeature",
                "markdown1",
                "",
                "markdown after empty line",
                "   Background:",
                "   markdown2",
                "   markdown3",
                "       Given a",
                "",
                "   Scenario: Scenario 1",
                "       indented markdown4",
                "   markdown5",
                "       Given a",
                "       When b",
                "       Then c",
                "",
                "   @tag",
                "   # not markdown",
                "   Scenario: Scenario 2",
                "   markdown6",
                "           double indented markdown7",
                "       Given a",
                "       When b",
                "       Then c",
                "",
                "   Scenario Outline: Scenario 3",
                "   markdown8",
                "   markdown9",
                "       Given a",
                "       When b",
                "       Then c",
                "# not markdown",
                "   Examples:",
                "   markdown10",
                "   markdown11",
                "       | header |",
                "       | value |",

            };
        }

        [Test]
        public void Should_parse_markdown_after_feature()
        {
            // When
            var result = new GherkinObjectParser(completeInputLines).Parse();

            // Then
            CollectionAssert.AreEqual(new List<string> 
            {
                "markdown1",
                "",
                "markdown after empty line"
            }, result.MarkdownLines);
        }

        [Test]
        public void Should_parse_markdown_after_background()
        {
            // When
            var result = new GherkinObjectParser(completeInputLines).Parse();

            // Then
            CollectionAssert.AreEqual(new List<string>
            {
                "markdown2",
                "markdown3",
            }, result.Background.MarkdownLines);
        }

        [Test]
        public void Should_parse_markdown_after_scenario1()
        {
            // When
            var result = new GherkinObjectParser(completeInputLines).Parse();

            // Then
            CollectionAssert.AreEqual(new List<string>
            {
                "    indented markdown4",
                "markdown5",
            }, result.Scenarios[0].MarkdownLines);
        }

        [Test]
        public void Should_parse_markdown_after_scenario2()
        {
            // When
            var result = new GherkinObjectParser(completeInputLines).Parse();

            // Then
            CollectionAssert.AreEqual(new List<string>
            {
                "markdown6",
                "        double indented markdown7",
            }, result.Scenarios[1].MarkdownLines);
        }

        [Test]
        public void Should_parse_markdown_after_scenario_outline()
        {
            // When
            var result = new GherkinObjectParser(completeInputLines).Parse();

            // Then
            CollectionAssert.AreEqual(new List<string>
            {
                "markdown8",
                "markdown9",
            }, result.Scenarios[2].MarkdownLines);
        }

        [Test]
        public void Should_parse_markdown_after_scenario_outline_examples()
        {
            // When
            var result = new GherkinObjectParser(completeInputLines).Parse();

            // Then
            CollectionAssert.AreEqual(new List<string>
            {
                "markdown10",
                "markdown11",
            }, result.Scenarios[2].MarkdownLinesExamples);
        }
    }
}
