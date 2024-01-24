using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GherkinSimpleParser
{
    public class GherkinObjectParser
    {
        private readonly List<string> inputLines;
        private readonly GherkinObject result;
        private readonly Stack<string> linesStack;

        private Scenario currentScenario;
        private FillingState fillingState;
        private int featureCount;
        private int backgroundCount;
        private List<string> lastSeenTags;
        private string currentLine;
        private int lineCount;
        private string TrimedLine => currentLine.Trim();

        public GherkinObjectParser(List<string> inputLines)
        {
            this.inputLines = inputLines;
            var inputLineReversed = new List<string>(inputLines);
            inputLineReversed.Reverse();
            linesStack = new Stack<string>(inputLineReversed);
            result = new GherkinObject();

            fillingState = FillingState.OTHER;
            featureCount = 0;
            backgroundCount = 0;
            lastSeenTags = new List<string>();
            lineCount = 0;
        }

        private enum FillingState
        {
            OTHER,
            BACKGROUND_GIVEN,
            SCENARIO_GIVEN,
            SCENARIO_WHEN,
            SCENARIO_THEN,
        }

        private class StateMachineException : Exception
        {
            public StateMachineException(string? message) : base(message)
            {
            }
        }

        public GherkinObject Parse()
        {
            while (linesStack.Count > 0)
            {
                currentLine = linesStack.Pop();
                lineCount++;

                if (TrimedLine.StartsWith("#") || string.IsNullOrEmpty(TrimedLine)) continue;
                else if (TrimedLine.StartsWith("@")) HandleTagLine();
                else if (TrimedLine.StartsWith("Feature: ")) HandleFeatureLine();
                else if (TrimedLine.StartsWith("Background:")) HandleBackgroundLine();
                else if (TrimedLine.StartsWith("Scenario: ")) HandleScenarioLine();
                else if (TrimedLine.StartsWith("Given ")) HandleGivenLine();
                else if (TrimedLine.StartsWith("When ")) HandleWhenLine();
                else if (TrimedLine.StartsWith("Then ")) HandleThenLine();
                else if (TrimedLine.StartsWith("And ")) HandleAndLine();
                else if (TrimedLine.StartsWith("\"\"\"")) HandleDocStringsBlock();
                else if (TrimedLine.StartsWith("|")) HandleDataTableBlock();
                else throw new ArgumentException($"Line {lineCount}: Unsupported line: \"{TrimedLine}\"", nameof(inputLines));
            }

            return result;
        }

        private void HandleDataTableBlock()
        {
            while (TrimedLine.StartsWith("|"))
            {
                var tableRow = TrimedLine.Split("|", StringSplitOptions.TrimEntries).Skip(1).SkipLast(1).ToList();
                switch (fillingState)
                {
                    case FillingState.OTHER:
                        break;
                    case FillingState.BACKGROUND_GIVEN:
                        break;
                    case FillingState.SCENARIO_GIVEN:
                        result.Scenarios.Last().Givens.Last().DataTable.Add(tableRow);
                        break;
                    case FillingState.SCENARIO_WHEN:
                        break;
                    case FillingState.SCENARIO_THEN:
                        break;
                    default:
                        break;
                }

                if (linesStack.Count == 0)
                    return;

                currentLine = linesStack.Pop();
                lineCount++;
            }
            linesStack.Push(currentLine);
        }

        private void HandleDocStringsBlock()
        {
            int indentCountReference = currentLine.IndexOf('"');
            currentLine = linesStack.Pop();
            lineCount++;
            while (!TrimedLine.StartsWith("\"\"\""))
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
                currentLine = linesStack.Pop();
                lineCount++;
            }
        }

        private void HandleAndLine()
        {
            if (fillingState == FillingState.SCENARIO_GIVEN)
                currentScenario.Givens.Add(new Instruction(TrimedLine.Substring(4)));
            else if (fillingState == FillingState.SCENARIO_THEN)
                currentScenario.Thens.Add(new Instruction(TrimedLine.Substring(4)));
            else if (fillingState == FillingState.BACKGROUND_GIVEN)
                result.Background.Givens.Add(new Instruction(TrimedLine.Substring(4)));
        }

        private void HandleThenLine()
        {
            currentScenario.Thens.Add(new Instruction(TrimedLine.Substring(5)));
            fillingState = FillingState.SCENARIO_THEN;
        }

        private void HandleWhenLine()
        {
            currentScenario.When = TrimedLine.Substring(5);
            fillingState = FillingState.SCENARIO_WHEN;
        }

        private void HandleGivenLine()
        {
            if (fillingState == FillingState.SCENARIO_GIVEN)
                currentScenario.Givens.Add(new Instruction(TrimedLine.Substring(6)));
            else if (fillingState == FillingState.BACKGROUND_GIVEN)
                result.Background.Givens.Add(new Instruction(TrimedLine.Substring(6)));
        }

        private void HandleScenarioLine()
        {
            currentScenario = new Scenario { Name = TrimedLine.Substring(10) };
            currentScenario.Tags = new List<string>(lastSeenTags.Distinct());
            lastSeenTags.Clear();
            result.Scenarios.Add(currentScenario);
            fillingState = FillingState.SCENARIO_GIVEN;
        }

        private void HandleBackgroundLine()
        {
            if (backgroundCount > 0)
                throw new ArgumentException($"Line {lineCount}: Do not support multiple Background in one file", nameof(inputLines));
            backgroundCount++;
            fillingState = FillingState.BACKGROUND_GIVEN;
        }

        private void HandleFeatureLine()
        {
            if (featureCount > 0)
                throw new ArgumentException($"Line {lineCount}: Do not support multiple features in one file", nameof(inputLines));
            result.FeatureName = TrimedLine.Substring(9);
            result.FeatureTags = new List<string>(lastSeenTags.Distinct());
            lastSeenTags.Clear();
            featureCount++;
            fillingState = FillingState.OTHER;
        }

        private void HandleTagLine()
        {
            if (TrimedLine.Split(' ').Any(sub => !sub.StartsWith("@")))
                throw new ArgumentException($"Line {lineCount}: In a tag line, spaces can only be used to separate tags starting with @.", nameof(inputLines));
            var tagsOnLine = TrimedLine.Replace(" ", "").Split('@', StringSplitOptions.RemoveEmptyEntries).Select(tag => "@" + tag);
            lastSeenTags.AddRange(tagsOnLine);
        }
    }
}
