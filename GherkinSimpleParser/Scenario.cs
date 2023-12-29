namespace GherkinSimpleParser
{
    public class Scenario
    {
        public string Name { get; set; }
        public List<string> Givens { get; set; } = new();
        public string When { get; set; }
        public List<string> Thens { get; set; } = new();
    }
}