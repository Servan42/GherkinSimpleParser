namespace GherkinSimpleParser
{
    public class Scenario
    {
        public string Name { get; set; }
        public List<Instruction> Givens { get; set; } = new();
        public string When { get; set; }
        public List<Instruction> Thens { get; set; } = new();
    }
}