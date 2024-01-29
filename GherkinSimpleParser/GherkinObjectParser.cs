using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
        private int markdownIndexIndent;
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
            MARKDOWN_FEATURE,
            MARKDOWN_BACKGROUND,
            MARKDOWN_SCENARIO,
            MARKDOWN_SCENARIO_OUTLINE_EXAMPLE,
            BACKGROUND_GIVEN,
            SCENARIO_GIVEN,
            SCENARIO_WHEN,
            SCENARIO_THEN,
            SCENARIO_OUTLINE_EXAMPLE,
        }

        public class StateMachineException : Exception
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

                if (TrimedLine.StartsWith("#")) continue;
                else if (string.IsNullOrEmpty(TrimedLine)
                    && fillingState != FillingState.MARKDOWN_FEATURE
                    && fillingState != FillingState.MARKDOWN_BACKGROUND
                    && fillingState != FillingState.MARKDOWN_SCENARIO
                    && fillingState != FillingState.MARKDOWN_SCENARIO_OUTLINE_EXAMPLE) continue;
                else if (TrimedLine.StartsWith("@")) HandleTagLine();
                else if (TrimedLine.StartsWith("Feature: ")) HandleFeatureLine();
                else if (TrimedLine.StartsWith("Background:")) HandleBackgroundLine();
                else if (TrimedLine.StartsWith("Scenario: ") || TrimedLine.StartsWith("Scenario Outline: ")) HandleScenarioLine();
                else if (TrimedLine.StartsWith("Given ")) HandleGivenLine();
                else if (TrimedLine.StartsWith("When ")) HandleWhenLine();
                else if (TrimedLine.StartsWith("Then ")) HandleThenLine();
                else if (TrimedLine.StartsWith("And ")) HandleAndLine();
                else if (TrimedLine.StartsWith("Examples:")) HandleExamplesBlock();
                else if (TrimedLine.StartsWith("\"\"\"")) HandleDocStringsBlock();
                else if (TrimedLine.StartsWith("|")) HandleDataTableBlock();
                else HandleMarkdownOrThrow();
            }

            AssertEveryScenarioOutlineHasExamples(result);

            return result;
        }

        private void AssertEveryScenarioOutlineHasExamples(GherkinObject result)
        {
            foreach (var scenario in result.Scenarios)
            {
                if (!scenario.IsScenarioOutline)
                    continue;

                if (scenario.Examples.Count == 0)
                    throw new ArgumentException("At least one Scenario Outline does not have Examples.", nameof(inputLines));

                foreach (var example in scenario.Examples.Values)
                {
                    if (example == null || example.Count == 0)
                        throw new ArgumentException("At least one Scenario Outline does not have Examples data.", nameof(inputLines));
                }
            }
        }

        private void HandleExamplesBlock()
        {
            if (currentScenario == null || currentScenario.IsScenarioOutline == false)
                throw new StateMachineException($"Line {lineCount}: Cannot have an Examples block for a Scenario that is not an outline.");

            fillingState = FillingState.MARKDOWN_SCENARIO_OUTLINE_EXAMPLE;
        }

        private void HandleMarkdownOrThrow()
        {
            string indentedLine = GetIndentedLine(markdownIndexIndent);
            switch (fillingState)
            {
                case FillingState.MARKDOWN_FEATURE:
                    result.MarkdownLines.Add(indentedLine);
                    break;
                case FillingState.MARKDOWN_BACKGROUND:
                    result.Background.MarkdownLines.Add(indentedLine);
                    break;
                case FillingState.MARKDOWN_SCENARIO:
                    currentScenario.MarkdownLines.Add(indentedLine);
                    break;
                case FillingState.MARKDOWN_SCENARIO_OUTLINE_EXAMPLE:
                    currentScenario.MarkdownLinesExamples.Add(indentedLine);
                    break;
                default:
                    throw new ArgumentException($"Line {lineCount}: Unsupported line: \"{TrimedLine}\"", nameof(inputLines));
            }
        }

        private void HandleDataTableBlock()
        {
            if (fillingState == FillingState.MARKDOWN_SCENARIO_OUTLINE_EXAMPLE) fillingState = FillingState.SCENARIO_OUTLINE_EXAMPLE;

            while (TrimedLine.StartsWith("|"))
            {
                var tableRow = TrimedLine.Split("|", StringSplitOptions.TrimEntries).Skip(1).SkipLast(1).ToList();
                switch (fillingState)
                {
                    case FillingState.BACKGROUND_GIVEN:
                        result.Background.Givens.Last().DataTable.Add(tableRow);
                        break;
                    case FillingState.SCENARIO_GIVEN:
                        currentScenario.Givens.Last().DataTable.Add(tableRow);
                        break;
                    case FillingState.SCENARIO_WHEN:
                        currentScenario.Whens.Last().DataTable.Add(tableRow);
                        break;
                    case FillingState.SCENARIO_THEN:
                        currentScenario.Thens.Last().DataTable.Add(tableRow);
                        break;
                    case FillingState.SCENARIO_OUTLINE_EXAMPLE:
                        AddTableRowToExamplesBlock(tableRow);
                        break;
                    default:
                        throw new StateMachineException($"Line {lineCount}: Pipe (|) cannot be handeled in {fillingState} state.");
                }

                if (linesStack.Count == 0)
                    return;

                currentLine = linesStack.Pop();
                lineCount++;
            }
            linesStack.Push(currentLine);
            lineCount--;
        }

        private void AddTableRowToExamplesBlock(List<string> tableRow)
        {
            if (currentScenario.Examples.Count == 0)
            {
                // Init headers
                if (tableRow.Distinct().Count() != tableRow.Count)
                    throw new ArgumentException($"Line {lineCount}: An Example cannot have duplicate headers", nameof(inputLines));
                tableRow.ForEach(c => currentScenario.Examples.Add(c, new List<string>()));
            }
            else
            {
                int i = 0;
                foreach (var column in currentScenario.Examples.Values)
                {
                    column.Add(tableRow[i]);
                    i++;
                }
            }
        }

        private void HandleDocStringsBlock()
        {
            int indentCountReference = currentLine.IndexOf('"');
            currentLine = linesStack.Pop();
            lineCount++;
            while (!TrimedLine.StartsWith("\"\"\""))
            {
                string indentedLine = GetIndentedLine(indentCountReference);
                switch (fillingState)
                {
                    case FillingState.BACKGROUND_GIVEN:
                        result.Background.Givens.Last().DocStrings.Add(indentedLine);
                        break;
                    case FillingState.SCENARIO_GIVEN:
                        currentScenario.Givens.Last().DocStrings.Add(indentedLine);
                        break;
                    case FillingState.SCENARIO_WHEN:
                        currentScenario.Whens.Last().DocStrings.Add(indentedLine);
                        break;
                    case FillingState.SCENARIO_THEN:
                        currentScenario.Thens.Last().DocStrings.Add(indentedLine);
                        break;
                    default:
                        throw new StateMachineException($"Line {lineCount}: Docstring (\"\"\") cannot be handeled in {fillingState} state.");
                }
                currentLine = linesStack.Pop();
                lineCount++;
            }
        }

        private void HandleAndLine()
        {
            switch (fillingState)
            {
                case FillingState.BACKGROUND_GIVEN:
                    result.Background.Givens.Add(new Instruction(TrimedLine.Substring(4)));
                    break;
                case FillingState.SCENARIO_GIVEN:
                    currentScenario.Givens.Add(new Instruction(TrimedLine.Substring(4)));
                    break;
                case FillingState.SCENARIO_WHEN:
                    currentScenario.Whens.Add(new Instruction(TrimedLine.Substring(4)));
                    break;
                case FillingState.SCENARIO_THEN:
                    currentScenario.Thens.Add(new Instruction(TrimedLine.Substring(4)));
                    break;
                default:
                    throw new StateMachineException($"Line {lineCount}: \"And\" cannot be handeled in {fillingState} state.");
            }
        }

        private void HandleThenLine()
        {
            currentScenario.Thens.Add(new Instruction(TrimedLine.Substring(5)));
            fillingState = FillingState.SCENARIO_THEN;
        }

        private void HandleWhenLine()
        {
            currentScenario.Whens.Add(new Instruction(TrimedLine.Substring(5)));
            fillingState = FillingState.SCENARIO_WHEN;
        }

        private void HandleGivenLine()
        {
            if (fillingState == FillingState.MARKDOWN_BACKGROUND) fillingState = FillingState.BACKGROUND_GIVEN;
            if (fillingState == FillingState.MARKDOWN_SCENARIO) fillingState = FillingState.SCENARIO_GIVEN;

            if (fillingState == FillingState.SCENARIO_GIVEN)
                currentScenario.Givens.Add(new Instruction(TrimedLine.Substring(6)));
            else if (fillingState == FillingState.BACKGROUND_GIVEN)
                result.Background.Givens.Add(new Instruction(TrimedLine.Substring(6)));
        }

        private void HandleScenarioLine()
        {
            currentScenario = new Scenario();

            if (TrimedLine.StartsWith("Scenario Outline: "))
            {
                currentScenario.Name = TrimedLine.Substring(18);
                currentScenario.IsScenarioOutline = true;
            }
            else
            {
                currentScenario.Name = TrimedLine.Substring(10);
                currentScenario.IsScenarioOutline = false;
            }

            currentScenario.Tags = new List<string>(lastSeenTags.Distinct());
            lastSeenTags.Clear();
            result.Scenarios.Add(currentScenario);
            markdownIndexIndent = currentLine.Replace("\t", "   ").TakeWhile(c => c == ' ').Count();
            fillingState = FillingState.MARKDOWN_SCENARIO;
        }

        private void HandleBackgroundLine()
        {
            if (backgroundCount > 0)
                throw new ArgumentException($"Line {lineCount}: Do not support multiple Background in one file", nameof(inputLines));
            backgroundCount++;
            markdownIndexIndent = currentLine.Replace("\t", "   ").TakeWhile(c => c == ' ').Count();
            fillingState = FillingState.MARKDOWN_BACKGROUND;
        }

        private void HandleFeatureLine()
        {
            if (featureCount > 0)
                throw new ArgumentException($"Line {lineCount}: Do not support multiple features in one file", nameof(inputLines));
            result.FeatureName = TrimedLine.Substring(9);
            result.FeatureTags = new List<string>(lastSeenTags.Distinct());
            lastSeenTags.Clear();
            featureCount++;
            markdownIndexIndent = currentLine.Replace("\t", "   ").TakeWhile(c => c == ' ').Count();
            fillingState = FillingState.MARKDOWN_FEATURE;
        }

        private void HandleTagLine()
        {
            if (TrimedLine.Split(' ').Any(sub => !sub.StartsWith("@")))
                throw new ArgumentException($"Line {lineCount}: In a tag line, spaces can only be used to separate tags starting with @.", nameof(inputLines));
            var tagsOnLine = TrimedLine.Replace(" ", "").Split('@', StringSplitOptions.RemoveEmptyEntries).Select(tag => "@" + tag);
            lastSeenTags.AddRange(tagsOnLine);
            fillingState = FillingState.OTHER;
        }

        private string GetIndentedLine(int firstCharIndexOnCurrentLineReference)
        {
            int firstCharIndex = string.IsNullOrEmpty(currentLine) ? 0 : currentLine.IndexOf(currentLine.Trim().First());
            int leadingZerosToAdd = firstCharIndex - firstCharIndexOnCurrentLineReference >= 0 ? firstCharIndex - firstCharIndexOnCurrentLineReference : 0;
            return new string(' ', leadingZerosToAdd) + currentLine.Trim();
        }
    }
}
