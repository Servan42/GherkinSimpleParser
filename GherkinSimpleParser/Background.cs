namespace GherkinSimpleParser
{
    public class Background
    {
        public List<string> MarkdownLines { get; set; } = new();
        public List<Instruction> Givens { get; set; } = new();
    }
}