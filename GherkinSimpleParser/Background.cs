namespace GherkinSimpleParser
{
    /// <summary>
    ///  Represents the Background block
    /// </summary>
    public class Background
    {
        /// <summary>
        /// The markdown lines that are right after the Background keyword line.
        /// </summary>
        public List<string> MarkdownLines { get; set; } = new();
        /// <summary>
        /// The Given instructions that are within the Background block
        /// </summary>
        public List<Instruction> Givens { get; set; } = new();
    }
}