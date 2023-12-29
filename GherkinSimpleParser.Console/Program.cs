using GherkinSimpleParser;
using GherkinSimpleParser.Converter;

try
{
    Console.WriteLine("Enter full path of input directory. Every .feature files inside of it will be parsed and exported");
	var inputDirectoryPath = Console.ReadLine();
	var outputDirPath = @$"{inputDirectoryPath}/GherkinToExcel";
	Directory.CreateDirectory(outputDirPath);

	var gherkinObjs = new List<GherkinObject>();
	foreach (var filepath in Directory.GetFiles(inputDirectoryPath).Where(f => f.Contains(".feature")))
	{
		Console.WriteLine(filepath);
		var lines = File.ReadAllLines(filepath).ToList();
		var obj = GherkinObject.Parse(lines);
		//File.WriteAllLines(@$"{outputDirPath}/{Path.GetFileName(filepath)}.csv", obj.ExportAsCSVWithExcelFormulaWrap_FR());
		gherkinObjs.Add(obj);
	}

	new ExcelConverter().AppendDataToExcelFile(Path.Combine(outputDirPath, "converted.xlsx"), gherkinObjs);
	Console.WriteLine($"Exported to {outputDirPath}");
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}
Console.WriteLine("Press any key to close...");
Console.ReadKey();