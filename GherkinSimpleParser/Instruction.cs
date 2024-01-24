namespace GherkinSimpleParser
{
    public class Instruction
    {
        public Instruction(string mainLine)
        {
            MainLine = mainLine;
        }

        public string MainLine { get; set; }
        public List<string> DocStrings { get; set; } = new();
        public GherkinDataTable DataTable { get; set; } = new();
    }
}