using GherkinSimpleParser;

var directory = Console.ReadLine();
Directory.CreateDirectory(@$"{directory}/GherkinToCsv");
foreach (var filepath in Directory.GetFiles(directory).Where(f => f.Contains(".feature")))
{
    Console.WriteLine(filepath);
    var lines = File.ReadAllLines(filepath).ToList();
    var goCSV = GherkinObject.Parse(lines).ExportAsCSVWithExcelFormulaWrap_FR();
    File.WriteAllLines(@$"{directory}/GherkinToCsv/{Path.GetFileName(filepath)}.csv", goCSV);
}