using System;
using System.Collections.Generic;
using System.IO;
using CSharpJExcel.Jxl;
using System.Linq;
using CSharpJExcel.Jxl.Write;
using CSharpJExcel.Jxl.Biff;

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