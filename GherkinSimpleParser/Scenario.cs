namespace GherkinSimpleParser
{
    public class Scenario
    {
        public string Name { get; set; }
        public List<Instruction> Givens { get; set; } = new();
        public List<Instruction> Whens { get; set; } = new();
        public List<Instruction> Thens { get; set; } = new();
        public List<string> Tags { get; set; }
    }
}