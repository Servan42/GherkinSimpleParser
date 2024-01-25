using System.Net;

namespace GherkinSimpleParser
{
    public class GherkinObject
    {
        public string FeatureName { get; set; }
        public List<string> MarkdownLines { get; set; } = new();
        public Background Background { get; set; } = new();
        public List<Scenario> Scenarios { get; set; } = new();
        public List<string> FeatureTags { get; set; }

        public Dictionary<string, List<Scenario>> GetScenariosByTag()
        {
            var result = new Dictionary<string, List<Scenario>>();

            foreach (var scenario in this.Scenarios)
            {
                foreach (var tag in scenario.Tags)
                {
                    if (result.ContainsKey(tag))
                        result[tag].Add(scenario);
                    else
                        result.Add(tag, new List<Scenario> { scenario });
                }
            }

            return result;
        }
    }
}