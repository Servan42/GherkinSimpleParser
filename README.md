# Description 

Library that loads [Gherkin syntax](https://cucumber.io/docs/gherkin/reference/) from a .feature file to an object in order to export it as formated text. This is not meant to interpret the language.

# Parsing

## How to use the library

```csharp
var inputLines = new List<string>();
var gherkinObj = GherkinObject.Parse(inputLines));
```

## Supports
* One `Feature` per file
* One `Background` per file
* `Scenario` (multiple)
* `Given`
* `And` (given, multiple)
* `When` (unique)
* `Then`
* `And` (then, multiple)

## Ignores
* Empty lines
* `#` (Comments)

## DO NOT support
* Guard clause for wrongly structured files
* Unexpected lines (will throw exception)
* Multiple `Feature` per file
* `Example`
* `But`
* `*`
* `Scenario Outline` (or `Scenario Template`)
* `Examples` (or `Scenarios`)
* `"""` (Doc Strings)
* `|` (Data Tables)
* `@` (Tags)

# Exports

## Export as Excel using EPPlus Excel library

Using the applicaiton extension `GherkinSimpleParser.Converter` you have access to the class `ExcelConverter` that exports the GherkinObject to a predefined Excel TestPlan.

## Export as CSV

### Export as CSV for testplan with \<speparator> for ANDs

Using the applicaiton extension `GherkinSimpleParser.Converter` you have access to the class `CSVConverter` that does the following:

```csharp
GherkinObject gherkinObj = GherkinObject.Parse(inputLines));
string separator = "|";
List<string> CSVLines = gherkinObj.ExportAsCSV(separator));
```

Transforms
```
Feature: feature name
# As user
# I want to do test cases
# In order to test

	Background:
		Given Prerequisite_0.1
		And Prere"q"uisite_0.2

	Scenario: Test Case 1
		Given Prerequisite_1.1
		And �Prere"q"uisite_1.2
		When Action_1
		Then Result_1.1
		And Resu"l"t_1.2

	Scenario: Test Case 2
		Given Prerequisite_2.1
		And Prere"q"uisite_2.2
		When Action_2
		Then Result_2.1
		And Resu"l"t_2.2
```
into
```
"Number;GIVEN;WHEN;THEN",
";GENERAL PREREQUISITES:|Prerequisite_0.1|Prere"q"uisite_0.2;;",
"1;Test Case 1;;",
";Prerequisite_1.1|Prere"q"uisite_1.2;Action_1;Result_1.1|Resu"l"t_1.2",
"2;Test Case 2;;",
";Prerequisite_2.1|Prere"q"uisite_2.2;Action_2;Result_2.1|Resu"l"t_2.2"
```

### Export as CSV for testplan in Excel with formula wrap

**LIMITATION: GIVENs and THENs text can only be 255 character longs because excel is annoying.**

```csharp
GherkinObject gherkinObj = GherkinObject.Parse(inputLines));
string separator = "|";
List<string> CSVLines = gherkinObj.ExportAsCSVWithExcelFormulaWrap_EN(separator));
```
Use `ExportAsCSVWithExcelFormulaWrap_FR` to have `CAR(10)` instead of `CHAR(10)` if you're french because Excel is annoying.

Transforms the above .feature file\
into
```
"Number;GIVEN;WHEN;THEN",
";="GENERAL PREREQUISITES:" & CHAR(10) & "Prerequisite_0.1" & CHAR(10) & "Prere""q""uisite_0.2";;",
"1;Test Case 1;;",
";="Prerequisite_1.1" & CHAR(10) & "Prere""q""uisite_1.2";Action_1;="Result_1.1" & CHAR(10) & "Resu""l""t_1.2"",
"2;Test Case 2;;",
";="Prerequisite_2.1" & CHAR(10) & "Prere""q""uisite_2.2";Action_2;="Result_2.1" & CHAR(10) & "Resu""l""t_2.2""            
```