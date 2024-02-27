

using System.Diagnostics.CodeAnalysis;

namespace GherkinSimpleParser
{
    /// <summary>
    /// Represents an instuction (Given, When, Then, And)
    /// </summary>
    public class Instruction
    {
        public Instruction(string mainLine)
        {
            MainLine = mainLine;
        }

        /// <summary>
        /// The text that is right after the Instruction (Given, When...) keyword. Does not include the keyword.
        /// </summary>
        public string MainLine { get; set; }
        /// <summary>
        /// The docstrings that are right after the Instruction line.
        /// </summary>
        public List<string> DocStrings { get; set; } = new();
        /// <summary>
        /// An object representing the DataTable that is right after the Instruction line.
        /// </summary>
        public GherkinDataTable DataTable { get; set; } = new();

        internal Instruction ResolveExampleValueInNewObject(Dictionary<string, List<string>> examples, int caseNumber)
        {
            var result = this.DeepClone(); 
            result.ResolveExampleValue(examples, caseNumber);
            return result;
        }

        private void ResolveExampleValue(Dictionary<string, List<string>> examples, int caseNumber)
        {
            foreach (var kvp in examples)
            {
                MainLine = MainLine.Replace($"<{kvp.Key}>", kvp.Value[caseNumber]);
                DocStrings = DocStrings.Select(ds => ds.Replace($"<{kvp.Key}>", kvp.Value[caseNumber])).ToList();
                DataTable = DataTable.ResolveExampleValueInNewObject(kvp.Key, kvp.Value[caseNumber]);
            }
        }

        private Instruction DeepClone()
        {
            return new Instruction(MainLine)
            {
                DocStrings = new List<string>(this.DocStrings),
                DataTable = this.DataTable.DeepClone()
            };
        }
    }
}