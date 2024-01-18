using System.Net;

namespace GherkinSimpleParser
{
    public class GherkinObject
    {
        public string FeatureName { get; set; }
        public Background Background { get; set; } = new();
        public List<Scenario> Scenarios { get; set; } = new();
        public string FeatureTag { get; set; }

        private enum FillingState
        {
            OTHER,
            BACKGROUND_GIVEN,
            SCENARIO_GIVEN,
            SCENARIO_THEN,
        }

        private class StateMachineException : Exception
        {
            public StateMachineException(string? message) : base(message)
            {
            }
        }

        public static GherkinObject Parse(List<string> inputLines)
        {
            var result = new GherkinObject();
            Scenario currentScenario = new();
            FillingState fillingState = FillingState.OTHER;
            int featureCount = 0;
            int backgroundCount = 0;
            string lastSeenTag = string.Empty;

            Queue<string> linesStack = new Queue<string>(inputLines);

            string currentLine;
            string trimedLine;
            int lineCount = 0;

            while (linesStack.Count > 0)
            {
                currentLine = linesStack.Dequeue();
                lineCount++;
                trimedLine = currentLine.Trim();

                if (trimedLine.StartsWith("#") || string.IsNullOrEmpty(trimedLine))
                {
                    continue;
                }
                else if(trimedLine.StartsWith("@"))
                {
                    if(!string.IsNullOrEmpty(lastSeenTag))
                        throw new ArgumentException($"Line {lineCount}: Encoutered new tag {trimedLine} but last tag {lastSeenTag} was not assigned to a feature of scenario.", nameof(inputLines));
                    lastSeenTag = trimedLine;
                }
                else if (trimedLine.StartsWith("Feature: "))
                {
                    if (featureCount > 0)
                        throw new ArgumentException($"Line {lineCount}: Do not support multiple features in one file", nameof(inputLines));
                    result.FeatureName = trimedLine.Substring(9);
                    result.FeatureTag = lastSeenTag;
                    lastSeenTag = string.Empty;
                    featureCount++;
                    fillingState = FillingState.OTHER;
                }
                else if (trimedLine.StartsWith("Background:"))
                {
                    if (backgroundCount > 0)
                        throw new ArgumentException($"Line {lineCount}: Do not support multiple Background in one file", nameof(inputLines));
                    backgroundCount++;
                    fillingState = FillingState.BACKGROUND_GIVEN;
                }
                else if (trimedLine.StartsWith("Scenario: "))
                {
                    currentScenario = new Scenario { Name = trimedLine.Substring(10) };
                    currentScenario.Tag = lastSeenTag;
                    lastSeenTag = string.Empty;
                    result.Scenarios.Add(currentScenario);
                    fillingState = FillingState.SCENARIO_GIVEN;
                }
                else if (trimedLine.StartsWith("Given "))
                {
                    if (fillingState == FillingState.SCENARIO_GIVEN)
                        currentScenario.Givens.Add(new Instruction(trimedLine.Substring(6)));
                    else if(fillingState == FillingState.BACKGROUND_GIVEN)
                        result.Background.Givens.Add(new Instruction(trimedLine.Substring(6)));
                }
                else if (trimedLine.StartsWith("When "))
                {
                    currentScenario.When = trimedLine.Substring(5);
                }
                else if (trimedLine.StartsWith("Then "))
                {
                    currentScenario.Thens.Add(new Instruction(trimedLine.Substring(5)));
                    fillingState = FillingState.SCENARIO_THEN;
                }
                else if (trimedLine.StartsWith("And "))
                {
                    if (fillingState == FillingState.SCENARIO_GIVEN)
                        currentScenario.Givens.Add(new Instruction(trimedLine.Substring(4)));
                    else if (fillingState == FillingState.SCENARIO_THEN)
                        currentScenario.Thens.Add(new Instruction(trimedLine.Substring(4)));
                    else if (fillingState == FillingState.BACKGROUND_GIVEN)
                        result.Background.Givens.Add(new Instruction(trimedLine.Substring(4)));
                }
                else if(trimedLine.StartsWith("\"\"\""))
                {
                    int indentCountReference = currentLine.IndexOf('"');
                    currentLine = linesStack.Dequeue();
                    lineCount++;
                    while (!currentLine.Trim().StartsWith("\"\"\""))
                    {
                        int firstCharIndex = string.IsNullOrEmpty(currentLine) ? 0 : currentLine.IndexOf(currentLine.Trim().First());
                        int leadingZerosToAdd = firstCharIndex - indentCountReference >= 0 ? firstCharIndex - indentCountReference : 0;
                        string indentedLine = new string(' ', leadingZerosToAdd) + currentLine.Trim();
                        switch (fillingState)
                        {
                            case FillingState.BACKGROUND_GIVEN:
                                result.Background.Givens.Last().DocStrings.Add(indentedLine);
                                break;
                            case FillingState.SCENARIO_GIVEN:
                                currentScenario.Givens.Last().DocStrings.Add(indentedLine);
                                break;
                            case FillingState.SCENARIO_THEN:
                                currentScenario.Thens.Last().DocStrings.Add(indentedLine);
                                break;
                            default:
                                throw new StateMachineException($"State {fillingState} is not allowed to contain DocString");
                        }
                        currentLine = linesStack.Dequeue();
                        lineCount++;
                    }
                }
                else
                {
                    throw new ArgumentException($"Line {lineCount}: Unsupported line: \"{trimedLine}\"", nameof(inputLines));
                }
            }

            return result;
        }

        public Dictionary<string, List<Scenario>> GetScenariosByTag()
        {
            var result = new Dictionary<string, List<Scenario>>();

            foreach (var scenario in this.Scenarios)
            {
                if (result.ContainsKey(scenario.Tag))
                    result[scenario.Tag].Add(scenario);
                else
                    result.Add(scenario.Tag, new List<Scenario> { scenario });
            }

            return result;
        }
    }
}