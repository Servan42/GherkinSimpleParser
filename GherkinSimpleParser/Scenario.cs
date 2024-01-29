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
    }
}