using System.Net;

namespace GherkinSimpleParser
{
    /// <summary>
    /// Represents a .feature file.
    /// </summary>
    public class GherkinObject
    {
        /// <summary>
        /// The name written after the Feature keyword.
        /// </summary>
        public string FeatureName { get; set; }
        /// <summary>
        /// The markdown lines that are right after the Feature keyword line.
        /// </summary>
        public List<string> MarkdownLines { get; set; } = new();
        /// <summary>
        /// Object that represents the Background block.
        /// </summary>
        public Background Background { get; set; } = new();
        /// <summary>
        /// List of objects that represents the Scenario blocks.
        /// </summary>
        public List<Scenario> Scenarios { get; set; } = new();
        /// <summary>
        /// The tags that are linked to the Feature keyword line.
        /// </summary>
        public List<string> FeatureTags { get; set; }

        /// <summary>
        /// Filters the scenarios in the GherkinObject and group them into a dictionary where the keys are the tags.
        /// </summary>
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

        /// <summary>
        /// For all ScenarioOutlines in the GherkinObject, transform them into normal Scenario objects where the variable of the outline have been resolved. 
        /// Replaces the Scenario object list with a new one where the ScenarioOutlines are removed and the resolved Scenarios are added.
        /// </summary>
        public void TransformScenarioOutlineToClassicScenarioAndOverrideScenarioList()
        {
            var newScenarioList = new List<Scenario>();

            foreach (var scenario in Scenarios)
            {
                if(!scenario.IsScenarioOutline)
                { 
                    newScenarioList.Add(scenario);
                    continue;
                }
                newScenarioList.AddRange(scenario.GetScenariosFromOutline());
            }
            this.Scenarios = newScenarioList;
        }
    }
}