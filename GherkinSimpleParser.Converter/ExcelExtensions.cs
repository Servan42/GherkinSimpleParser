using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GherkinSimpleParser.Converter
{
    public static class ExcelExtensions
    {
        public static void SetGridBorder(this ExcelWorksheet ws, ExcelBorderStyle borderStyle, int startRow, int endRow, int startCol, int endCol)
        {
            for (int row = startRow; row <= endRow; row++)
            {
                for (int col = startCol; col <= endCol; col++)
                {
                    GetCellAt(ws, row, col).Style.Border.BorderAround(borderStyle);
                }
            }
        }

        public static ExcelRange GetCellAt(this ExcelWorksheet ws, int row, int col)
        {
            char colChar = (char)('A' - 1 + col);
            return ws.Cells[$"{colChar}{row}"];
        }

        public static void SetColor(this ExcelWorksheet ws, string range, Color color)
        {
            ws.Cells[range].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[range].Style.Fill.BackgroundColor.SetColor(color);
        }

        public static void AlignCenter(this ExcelWorksheet ws, string range, bool vertical, bool horizontal)
        {
            if (horizontal)
                ws.Cells[range].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            if (vertical)
                ws.Cells[range].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        }
    }
}
