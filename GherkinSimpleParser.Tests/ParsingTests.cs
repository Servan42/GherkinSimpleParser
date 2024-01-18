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
            Assert.That(result.Scenarios.First().Givens.First().MainLine, Is.EqualTo("prerequisite"));
            Assert.That(result.Scenarios.Last().Givens.First().MainLine, Is.EqualTo("prerequisite2"));
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
            Assert.That(result.Scenarios.First().Givens.Last().MainLine, Is.EqualTo("prerequisite1"));
            Assert.That(result.Scenarios.Last().Givens.Last().MainLine, Is.EqualTo("prerequisite3"));
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
            Assert.That(result.Scenarios.First().Thens.First().MainLine, Is.EqualTo("result"));
            Assert.That(result.Scenarios.Last().Thens.First().MainLine, Is.EqualTo("result2"));
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
            Assert.That(result.Scenarios.First().Thens.Last().MainLine, Is.EqualTo("result1"));
            Assert.That(result.Scenarios.Last().Thens.Last().MainLine, Is.EqualTo("result3"));
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
                Assert.That(result.Background.Givens.First().MainLine, Is.EqualTo("prerequisite"));
                Assert.That(result.Background.Givens.Last().MainLine, Is.EqualTo("prerequisite1"));
                Assert.That(result.Scenarios.First().Givens.First().MainLine, Is.EqualTo("prerequisite2"));
                Assert.That(result.Scenarios.First().Givens.Last().MainLine, Is.EqualTo("prerequisite3"));
                Assert.That(result.Scenarios.First().Thens.First().MainLine, Is.EqualTo("result"));
                Assert.That(result.Scenarios.Last().Thens.Last().MainLine, Is.EqualTo("result1"));
            });
        }

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
            var result = GherkinObject.Parse(inputLines);

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
            var result = GherkinObject.Parse(inputLines);

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
            var result = GherkinObject.Parse(inputLines);

            // Then
            Assert.That(result.FeatureTag, Is.EqualTo("@featuretag"));
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
                "   @scenarioTag 1",
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
            var result = GherkinObject.Parse(inputLines);

            // Then
            Assert.That(result.Scenarios.First().Tag, Is.EqualTo("@scenarioTag 1"));
            Assert.That(result.Scenarios.Last().Tag, Is.EqualTo("@scenarioTag2"));
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
                "   @scenarioTag 1",
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
                "   @scenarioTag 1",
                "   Scenario: Scenario 3",
                "       Given a",
                "       When b",
                "       Then c",
            };

            var gherkinObject = GherkinObject.Parse(inputLines);

            // When
            var dict = gherkinObject.GetScenariosByTag();

            // Then
            Assert.That(dict.Count, Is.EqualTo(2));
            Assert.That(dict["@scenarioTag 1"].Count, Is.EqualTo(2));
            Assert.That(dict["@scenarioTag 1"].First().Name, Is.EqualTo("Scenario 1"));
            Assert.That(dict["@scenarioTag 1"].Last().Name, Is.EqualTo("Scenario 3"));
            Assert.That(dict["@scenarioTag2"].Count, Is.EqualTo(1));
            Assert.That(dict["@scenarioTag2"].Last().Name, Is.EqualTo("Scenario 2"));
        }

        [TestCase("Example:")]
        [TestCase("But")]
        [TestCase("*")]
        [TestCase("Scenario Outline:")]
        [TestCase("Scenario Template:")]
        [TestCase("Examples:")]
        [TestCase("Scenarios:")]
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