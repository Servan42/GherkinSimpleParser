using System.Net;

namespace GherkinSimpleParser
{
    public class GherkinObject
    {
        public string FeatureName { get; set; }
        public Background Background { get; set; } = new();
        public List<Scenario> Scenarios { get; set; } = new();

        public static GherkinObject Parse(List<string> inputLines)
        {
            var result = new GherkinObject();
            Scenario currentScenario = new();
            string andFillingState = "";
            int featureCount = 0;
            int backgroundCount = 0;
            int lineCount = 0;

            foreach (var line in inputLines.Select(l => l.Trim()))
            {
                lineCount++;
                if (line.StartsWith("#") || string.IsNullOrEmpty(line))
                {
                    continue;
                }
                else if (line.StartsWith("Feature: "))
                {
                    if (featureCount > 0)
                        throw new ArgumentException($"Line {lineCount}: Do not support multiple features in one file", nameof(inputLines));
                    result.FeatureName = line.Substring(9);
                    featureCount++;
                }
                else if (line.StartsWith("Background:"))
                {
                    if (backgroundCount > 0)
                        throw new ArgumentException($"Line {lineCount}: Do not support multiple Background in one file", nameof(inputLines));
                    backgroundCount++;
                }
                else if (line.StartsWith("Scenario: "))
                {
                    currentScenario = new Scenario { Name = line.Substring(10) };
                    result.Scenarios.Add(currentScenario);
                    andFillingState = "SCENARIO_GIVEN";
                }
                else if (line.StartsWith("Given "))
                {
                    if (andFillingState == "SCENARIO_GIVEN")
                        currentScenario.Givens.Add(line.Substring(6));
                    else
                        result.Background.Givens.Add(line.Substring(6));
                }
                else if (line.StartsWith("When "))
                {
                    currentScenario.When = line.Substring(5);
                }
                else if (line.StartsWith("Then "))
                {
                    currentScenario.Thens.Add(line.Substring(5));
                    andFillingState = "SCENARIO_THEN";
                }
                else if (line.StartsWith("And "))
                {
                    if (andFillingState == "SCENARIO_GIVEN")
                        currentScenario.Givens.Add(line.Substring(4));
                    else if (andFillingState == "SCENARIO_THEN")
                        currentScenario.Thens.Add(line.Substring(4));
                    else
                        result.Background.Givens.Add(line.Substring(4));
                }
                else
                {
                    throw new ArgumentException($"Line {lineCount}: Unsupported line: \"{line}\"", nameof(inputLines));
                }
            }

            return result;
        }
    }
}