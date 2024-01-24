using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GherkinSimpleParser.Tests.ParsingTests
{
    internal class ParsingDataTablesTests
    {
        private List<string> completeInputLines;

        [SetUp]
        public void Setup()
        {
            completeInputLines = new List<string>
            {
                "   Background:",
                "       Given prerequisite",
                "       | bg1.1 | bg2.1 | bg3.1 |",
                "       | bg4.1 | bg5.1 | bg6.1 |",
                "       And prerequisite1",
                "       | bg1.2 | bg2.2 | bg3.2 |",
                "       | bg4.2 | bg5.2 | bg6.2 |",
                "   Scenario: Should do something",
                "       Given prerequisite2",
                "       | name   | email              | twitter         |",
                "       | Aslak  | aslak@cucumber.io  | @aslak_hellesoy |",
                "       | Alice  |   | @alice |",
                "       And prerequisite3",
                "       | 1   | 2              | 3         |",
                "       |4    |    5|6|",
                "       When action1",
                "       | w1.1 | w2.1 | w3.1 |",
                "       | w4.1 | w5.1 | w6.1 |",
                "       And action2",
                "       | w1.2 | w2.2 | w3.2 |",
                "       | w4.2 | w5.2 | w6.2 |",
                "       Then result",
                "       | t1.1 | t2.1 | t3.1 |",
                "       | t4.1 | t5.1 | t6.1 |",
                "       And result1",
                "       | t1.2 | t2.2 | t3.2 |",
                "       | t4.2 | t5.2 | t6.2 |"
            };
        }

        [Test]
        public void Should_parse_a_data_table_for_background_given()
        {
            // When
            var result = new GherkinObjectParser(completeInputLines).Parse();

            // Then
            var instruciton = result.Background.Givens[0];
            Assert.That(instruciton.MainLine, Is.EqualTo("prerequisite"));
            CollectionAssert.AreEqual(new List<List<string>>
            {
                new List<string>{ "bg1.1", "bg2.1", "bg3.1" },
                new List<string>{ "bg4.1", "bg5.1", "bg6.1" },
            }, instruciton.DataTable);
        }

        [Test]
        public void Should_parse_a_data_table_for_background_given_and()
        {
            // When
            var result = new GherkinObjectParser(completeInputLines).Parse();

            // Then
            var instruciton = result.Background.Givens[1];
            Assert.That(instruciton.MainLine, Is.EqualTo("prerequisite1"));
            CollectionAssert.AreEqual(new List<List<string>>
            {
                new List<string>{ "bg1.2", "bg2.2", "bg3.2" },
                new List<string>{ "bg4.2", "bg5.2", "bg6.2" },
            }, instruciton.DataTable);
        }

        [Test]
        public void Should_parse_a_data_table_for_scenario_given()
        {
            // When
            var result = new GherkinObjectParser(completeInputLines).Parse();

            // Then
            var instruciton = result.Scenarios[0].Givens[0];
            Assert.That(instruciton.MainLine, Is.EqualTo("prerequisite2"));
            CollectionAssert.AreEqual(new List<List<string>>
            {
                new List<string>{ "name", "email", "twitter" },
                new List<string>{ "Aslak", "aslak@cucumber.io", "@aslak_hellesoy" },
                new List<string>{ "Alice", "", "@alice" }
            }, instruciton.DataTable);
        }

        [Test]
        public void Should_parse_a_data_table_for_scenario_given_and()
        {
            // When
            var result = new GherkinObjectParser(completeInputLines).Parse();

            // Then
            var instruciton = result.Scenarios[0].Givens[1];
            Assert.That(instruciton.MainLine, Is.EqualTo("prerequisite3"));
            CollectionAssert.AreEqual(new List<List<string>>
            {
                new List<string>{ "1", "2", "3" },
                new List<string>{ "4", "5", "6" },
            }, instruciton.DataTable);
        }

        [Test]
        public void Should_parse_a_data_table_for_scenario_when()
        {
            // When
            var result = new GherkinObjectParser(completeInputLines).Parse();

            // Then
            var instruciton = result.Scenarios[0].Whens[0];
            Assert.That(instruciton.MainLine, Is.EqualTo("action1"));
            CollectionAssert.AreEqual(new List<List<string>>
            {
                new List<string>{ "w1.1", "w2.1", "w3.1" },
                new List<string>{ "w4.1", "w5.1", "w6.1" },
            }, instruciton.DataTable);
        }

        [Test]
        public void Should_parse_a_data_table_for_scenario_when_and()
        {
            // When
            var result = new GherkinObjectParser(completeInputLines).Parse();

            // Then
            var instruciton = result.Scenarios[0].Whens[1];
            Assert.That(instruciton.MainLine, Is.EqualTo("action2"));
            CollectionAssert.AreEqual(new List<List<string>>
            {
                new List<string>{ "w1.2", "w2.2", "w3.2" },
                new List<string>{ "w4.2", "w5.2", "w6.2" },
            }, instruciton.DataTable);
        }

        [Test]
        public void Should_parse_a_data_table_for_scenario_then()
        {
            // When
            var result = new GherkinObjectParser(completeInputLines).Parse();

            // Then
            var instruciton = result.Scenarios[0].Thens[0];
            Assert.That(instruciton.MainLine, Is.EqualTo("result"));
            CollectionAssert.AreEqual(new List<List<string>>
            {
                new List<string>{ "t1.1", "t2.1", "t3.1" },
                new List<string>{ "t4.1", "t5.1", "t6.1" },
            }, instruciton.DataTable);
        }

        [Test]
        public void Should_parse_a_data_table_for_scenario_then_and()
        {
            // When
            var result = new GherkinObjectParser(completeInputLines).Parse();

            // Then
            var instruciton = result.Scenarios[0].Thens[1];
            Assert.That(instruciton.MainLine, Is.EqualTo("result1"));
            CollectionAssert.AreEqual(new List<List<string>>
            {
                new List<string>{ "t1.2", "t2.2", "t3.2" },
                new List<string>{ "t4.2", "t5.2", "t6.2" },
            }, instruciton.DataTable);
        }
    }
}
