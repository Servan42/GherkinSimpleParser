namespace GherkinSimpleParser.Tests
{
    public class ParsingTests
    {
        [Test]
        public void Should_parse_feature_name()
        {
            // Given
            var inputLines = new List<string>
            {
                "Feature: myFeature"
            };

            // When
            var result = GherkinObject.Parse(inputLines);

            // Then
            Assert.That(result.FeatureName, Is.EqualTo("myFeature"));
        }

        [Test]
        public void Should_parse_Scenario_name()
        {
            // Given
            var inputLines = new List<string>
            {
                "   Scenario: Should do something"
            };

            // When
            var result = GherkinObject.Parse(inputLines);

            // Then
            Assert.That(result.Scenarios.First().Name, Is.EqualTo("Should do something"));
        }

        [Test]
        public void Should_parse_given_for_scenario()
        {
            // Given
            var inputLines = new List<string>
            {
                "   Scenario: Should do something",
                "       Given prerequisite",
                "   Scenario: Should do something else",
                "       Given prerequisite2"
            };

            // When
            var result = GherkinObject.Parse(inputLines);

            // Then
            Assert.That(result.Scenarios.First().Givens.First(), Is.EqualTo("prerequisite"));
            Assert.That(result.Scenarios.Last().Givens.First(), Is.EqualTo("prerequisite2"));
        }

        [Test]
        public void Should_parse_given_ands_for_scenario()
        {
            // Given
            var inputLines = new List<string>
            {
                "   Scenario: Should do something",
                "       Given prerequisite",
                "       And prerequisite1",
                "   Scenario: Should do something else",
                "       Given prerequisite2",
                "       And prerequisite3",
            };

            // When
            var result = GherkinObject.Parse(inputLines);

            // Then
            Assert.That(result.Scenarios.First().Givens.Last(), Is.EqualTo("prerequisite1"));
            Assert.That(result.Scenarios.Last().Givens.Last(), Is.EqualTo("prerequisite3"));
        }

        [Test]
        public void Should_parse_given_when_for_scenario()
        {
            // Given
            var inputLines = new List<string>
            {
                "   Scenario: Should do something",
                "       When action",
                "   Scenario: Should do something else",
                "       When action2",
            };

            // When
            var result = GherkinObject.Parse(inputLines);

            // Then
            Assert.That(result.Scenarios.First().When, Is.EqualTo("action"));
            Assert.That(result.Scenarios.Last().When, Is.EqualTo("action2"));
        }

        [Test]
        public void Should_parse_then_for_scenario()
        {
            // Given
            var inputLines = new List<string>
            {
                "   Scenario: Should do something",
                "       Then result",
                "   Scenario: Should do something else",
                "       Then result2"
            };

            // When
            var result = GherkinObject.Parse(inputLines);

            // Then
            Assert.That(result.Scenarios.First().Thens.First(), Is.EqualTo("result"));
            Assert.That(result.Scenarios.Last().Thens.First(), Is.EqualTo("result2"));
        }

        [Test]
        public void Should_parse_then_ands_for_scenario()
        {
            // Given
            var inputLines = new List<string>
            {
                "   Scenario: Should do something",
                "       Then result",
                "       And result1",
                "",
                "   Scenario: Should do something else",
                "       Then result2",
                "       And result3",
            };

            // When
            var result = GherkinObject.Parse(inputLines);

            // Then
            Assert.That(result.Scenarios.First().Thens.Last(), Is.EqualTo("result1"));
            Assert.That(result.Scenarios.Last().Thens.Last(), Is.EqualTo("result3"));
        }

        [Test]
        public void Should_parse_background_and_not_mistaken_the_ands()
        {
            // Given
            var inputLines = new List<string>
            {
                "   Background:",
                "       Given prerequisite",
                "       And prerequisite1",
                "   Scenario: Should do something",
                "       Given prerequisite2",
                "       And prerequisite3",
                "       When action",
                "       Then result",
                "       And result1",
            };

            // When
            var result = GherkinObject.Parse(inputLines);

            // Then
            Assert.Multiple(() =>
            {
                Assert.That(result.Background.Givens.First(), Is.EqualTo("prerequisite"));
                Assert.That(result.Background.Givens.Last(), Is.EqualTo("prerequisite1"));
                Assert.That(result.Scenarios.First().Givens.First(), Is.EqualTo("prerequisite2"));
                Assert.That(result.Scenarios.First().Givens.Last(), Is.EqualTo("prerequisite3"));
                Assert.That(result.Scenarios.First().Thens.First(), Is.EqualTo("result"));
                Assert.That(result.Scenarios.Last().Thens.Last(), Is.EqualTo("result1"));
            });
        }

        [TestCase("Example:")]
        [TestCase("But")]
        [TestCase("*")]
        [TestCase("Scenario Outline:")]
        [TestCase("Scenario Template:")]
        [TestCase("Examples:")]
        [TestCase("Scenarios:")]
        [TestCase("\"\"\"")]
        [TestCase("|")]
        [TestCase("@")]
        public void Should_throw_exception_when_encounters_an_unsupported_line(string startWith)
        {
            // Given
            var inputLines = new List<string>
            {
                $"{startWith} anything",
            };

            // When
            Assert.Throws<ArgumentException>(() => GherkinObject.Parse(inputLines));
        }

        [Test]
        public void Should_throw_exception_when_encounters_a_second_feature()
        {
            // Given
            var inputLines = new List<string>
            {
                $"Feature: feature1",
                $"Feature: feature2",
            };

            // When
            Assert.Throws<ArgumentException>(() => GherkinObject.Parse(inputLines));
        }
    }
}