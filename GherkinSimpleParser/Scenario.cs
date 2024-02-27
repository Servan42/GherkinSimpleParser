using System.Collections;

namespace GherkinSimpleParser
{
    /// <summary>
    /// Represents the Scenario block
    /// </summary>
    public class Scenario
    {
        /// <summary>
        /// The name written after the Scenario keyword.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The markdown lines that are right after the Scenario keyword line.
        /// </summary>
        public List<string> MarkdownLines { get; set; } = new();
        /// <summary>
        /// The markdown lines that are right after the Examples keyword line.
        /// </summary>
        public List<string> MarkdownLinesExamples { get; set; } = new();
        /// <summary>
        /// The Given instructions that are within the Scenario block
        /// </summary>
        public List<Instruction> Givens { get; set; } = new();
        /// <summary>
        /// The When instructions that are within the Scenario block
        /// </summary>
        public List<Instruction> Whens { get; set; } = new();
        /// <summary>
        /// The Then instructions that are within the Scenario block
        /// </summary>
        public List<Instruction> Thens { get; set; } = new();
        /// <summary>
        /// The tags that are linked to the Scenario keyword line.
        /// </summary>
        public List<string> Tags { get; set; }
        /// <summary>
        /// Says wether this scenario is a scenario outline or no.
        /// </summary>
        public bool IsScenarioOutline { get; set; } = false;
        /// <summary>
        /// If this scenario is a scenario Outline, this collection represents the Examples block under the Scenario block.
        /// </summary>
        public Dictionary<string, List<string>> Examples { get; set; } = new();

        /// <summary>
        /// If this scenario is a ScenarioOutline, get the variables present in the Examples block and returns multiple scenarios with the variables resolved.
        /// </summary>
        public List<Scenario> GetScenariosFromOutline()
        {
            if (!this.IsScenarioOutline)
                throw new InvalidOperationException($"The scenario {this.Name} is not a Scenario Outline.");

            var createdScenarios = new List<Scenario>();

            for (int caseNumber = 0; caseNumber < Examples.First().Value.Count; caseNumber++)
            {
                var scenario = new Scenario()
                {
                    Name = $"{this.Name} Example {caseNumber + 1}",
                    MarkdownLines = new List<string>(this.MarkdownLines),
                    Givens = this.Givens.Select(i => i.ResolveExampleValueInNewObject(Examples, caseNumber)).ToList(),
                    Whens = this.Whens.Select(i => i.ResolveExampleValueInNewObject(Examples, caseNumber)).ToList(),
                    Thens = this.Thens.Select(i => i.ResolveExampleValueInNewObject(Examples, caseNumber)).ToList(),
                    Tags = new List<string>(this.Tags),
                    IsScenarioOutline = false
                };
                createdScenarios.Add(scenario);
            }

            return createdScenarios;
        }
    }
}