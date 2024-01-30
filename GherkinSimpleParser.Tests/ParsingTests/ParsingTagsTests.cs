using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GherkinSimpleParser.Tests.ParsingTests
{
    internal class ParsingTagsTests
    {
        [Test]
        public void Should_parse_tag_before_feature()
        {
            // Given
            var inputLines = new List<string>
            {
                "@featuretag",
                "Feature: myFeature"
            };

            // When
            var result = new GherkinObjectParser(inputLines).Parse();

            // Then
            Assert.That(result.FeatureTags[0], Is.EqualTo("@featuretag"));
        }

        [Test]
        public void Should_parse_tag_before_scenarios()
        {
            // Given
            var inputLines = new List<string>
            {
                "@featuretag",
                "Feature: myFeature",
                "",
                "   @scenarioTag1",
                "   Scenario: Scenario 1",
                "       Given a",
                "       When b",
                "       Then c",
                "",
                "   @scenarioTag2",
                "   Scenario: Scenario 2",
                "       Given a",
                "       When b",
                "       Then c",
            };

            // When
            var result = new GherkinObjectParser(inputLines).Parse();

            // Then
            Assert.That(result.Scenarios.First().Tags[0], Is.EqualTo("@scenarioTag1"));
            Assert.That(result.Scenarios.Last().Tags[0], Is.EqualTo("@scenarioTag2"));
        }

        [Test]
        public void Should_parse_multiple_tags_per_scenarios_or_feature()
        {
            // Given
            var inputLines = new List<string>
            {
                "@featuretag0",
                "@featuretag1 @featuretag2",
                "@featuretag3@featuretag4",
                "@featuretag0",
                "Feature: myFeature",
                "",
                "   @scenarioTag0",
                "   @scenarioTag1 @scenarioTag2",
                "   @scenarioTag3@scenarioTag4",
                "   @scenarioTag0",
                "   Scenario: Scenario 1",
                "       Given a",
                "       When b",
                "       Then c",
            };

            // When
            var result = new GherkinObjectParser(inputLines).Parse();

            // Then
            CollectionAssert.AreEquivalent(new List<string> { "@featuretag0", "@featuretag1", "@featuretag2", "@featuretag3", "@featuretag4" }, result.FeatureTags);
            CollectionAssert.AreEquivalent(new List<string> { "@scenarioTag0", "@scenarioTag1", "@scenarioTag2", "@scenarioTag3", "@scenarioTag4" }, result.Scenarios.First().Tags);
        }

        [TestCase("@no space in tag", "@correct")]
        [TestCase("@correct", "@no space in tag")]
        public void Should_parse_throw_exception_when_tags_are_incorrect(string featureTagLine, string scenarioTagLine)
        {
            // Given
            var inputLines = new List<string>
            {
                featureTagLine,
                "Feature: myFeature",
                "",
                scenarioTagLine,
                "   Scenario: Scenario 1",
                "       Given a",
                "       When b",
                "       Then c",
            };

            // When/Then
            Assert.Throws<ArgumentException>(() => new GherkinObjectParser(inputLines).Parse());
        }

        [Test]
        public void Should_get_scenario_tag_dictionnary()
        {
            // Given
            var inputLines = new List<string>
            {
                "@featuretag",
                "Feature: myFeature",
                "",
                "   @scenarioTag1",
                "   Scenario: Scenario 1",
                "       Given a",
                "       When b",
                "       Then c",
                "",
                "   @scenarioTag2",
                "   Scenario: Scenario 2",
                "       Given a",
                "       When b",
                "       Then c",
                "",
                "   @scenarioTag1",
                "   Scenario: Scenario 3",
                "       Given a",
                "       When b",
                "       Then c",
            };

            var gherkinObject = new GherkinObjectParser(inputLines).Parse();

            // When
            var dict = gherkinObject.GetScenariosByTag();

            // Then
            Assert.That(dict.Count, Is.EqualTo(2));
            Assert.That(dict["@scenarioTag1"].Count, Is.EqualTo(2));
            Assert.That(dict["@scenarioTag1"].First().Name, Is.EqualTo("Scenario 1"));
            Assert.That(dict["@scenarioTag1"].Last().Name, Is.EqualTo("Scenario 3"));
            Assert.That(dict["@scenarioTag2"].Count, Is.EqualTo(1));
            Assert.That(dict["@scenarioTag2"].Last().Name, Is.EqualTo("Scenario 2"));
        }
    }
}
