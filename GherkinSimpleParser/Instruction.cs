

using System.Diagnostics.CodeAnalysis;

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