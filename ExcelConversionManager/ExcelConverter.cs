using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelConversionManager
{
    public class ExcelConverter
    {
        public static void Init()
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var stream = new MemoryStream();
            using (var excelPack = new ExcelPackage(stream))
            {
                var ws = excelPack.Workbook.Worksheets.Add("sheetName");
                ws.Cells["D4"].Value = "Value for D4";
                ws.Cells["D4"].Style.Font.Bold = true;
                ws.Cells["D4:D10"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                ws.Cells["D4:D10"].Style.Fill.BackgroundColor.SetColor(1, 255, 34, 145);
                
                excelPack.SaveAs(stream);
            }

            stream.Position = 0;
            File.WriteAllBytes("output.xlsx", stream.ToArray());
        }
    }
}
