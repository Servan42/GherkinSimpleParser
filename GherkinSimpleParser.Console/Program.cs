using GherkinSimpleParser;

var directory = Console.ReadLine();
Directory.CreateDirectory(@$"{directory}/GherkinToExcel");

var gos = new List<GherkinObject>();
foreach (var filepath in Directory.GetFiles(directory).Where(f => f.Contains(".feature")))
{
    Console.WriteLine(filepath);
    var lines = File.ReadAllLines(filepath).ToList();
    gos.Add(GherkinObject.Parse(lines));
    //File.WriteAllLines(@$"{directory}/GherkinToCsv/{Path.GetFileName(filepath)}.csv", goCSV);
}

var excelConversionManager = new ExcelConversionManager();
excelConversionManager.AppendTestsToExcelFile(@$"{directory}/GherkinToCsv/converted", gos);