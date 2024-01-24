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
                "       | 19 | 20 | 21 |",
                "       | 22 | 23 | 24 |",
                "       And prerequisite1",
                "       | 25 | 26 | 27 |",
                "       | 28 | 29 | 30 |",
                "   Scenario: Should do something",
                "       Given prerequisite2",
                "       | name   | email              | twitter         |",
                "       | Aslak  | aslak@cucumber.io  | @aslak_hellesoy |",
                "       | Alice  |   | @alice |",
                "       And prerequisite3",
                "       | 1   | 2              | 3         |",
                "       | 4  | 5  | 6 |",
                "       When action",
                "       | 31 | 32 | 33 |",
                "       | 34 | 35 | 36 |",
                "       Then result",
                "       |7|8 | 9|",
                "       | 10 | 11 | 12 |",
                "       And result1",
                "       | 13 | 14 | 15 |",
                "       | 16 | 17 | 18 |",
            };
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
    }
}
