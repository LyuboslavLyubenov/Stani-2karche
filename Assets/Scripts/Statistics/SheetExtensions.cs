using System;

using CSharpJExcel.Jxl;
using CSharpJExcel.Jxl.Biff;

namespace Assets.Scripts.Statistics
{

    public static class SheetExtensions
    {
        public static Cell GetCellOrDefault(this Sheet sheet, int column, int row)
        {
            Cell cell;

            try
            {
                cell = sheet.getCell(column, row);    
            }
            catch (Exception ex)
            {
                cell = new EmptyCell(column, row);
            }

            return cell;
        }
    }

}