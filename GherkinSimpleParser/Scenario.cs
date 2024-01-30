using System.Collections;

namespace GherkinSimpleParser
{
    public class Scenario
    {
        public string Name { get; set; }
        public List<string> MarkdownLines { get; set; } = new();
        public List<string> MarkdownLinesExamples { get; set; } = new();
        public List<Instruction> Givens { get; set; } = new();
        public List<Instruction> Whens { get; set; } = new();
        public List<Instruction> Thens { get; set; } = new();
        public List<string> Tags { get; set; }
        public bool IsScenarioOutline { get; set; } = false;
        public Dictionary<string, List<string>> Examples { get; set; } = new();

        internal List<Scenario> GetScenariosFromOutline()
        {
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