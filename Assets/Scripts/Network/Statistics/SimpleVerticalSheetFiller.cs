using System;
using System.Collections.Generic;
using System.IO;
using CSharpJExcel.Jxl;
using UnityEngine;
using System.Linq;
using CSharpJExcel.Jxl.Write;
using CSharpJExcel.Jxl.Biff;

public class SimpleVerticalSheetFiller : ISheetFiller
{
    int startColumn;
    int startRow;

    public SimpleVerticalSheetFiller(int startColumn, int startRow)
    {
        this.startColumn = startColumn;
        this.startRow = startRow;
    }

    public void Fill(WritableSheet s, Dictionary<string, string> dataHeaderValues)
    {
        var dataHeaderValuesKeyPair = dataHeaderValues.ToArray();

        for (int i = 0; i < dataHeaderValuesKeyPair.Length; i++)
        {
            var dataHeaderValue = dataHeaderValuesKeyPair[i];
            var header = dataHeaderValue.Key;
            var dataValue = dataHeaderValue.Value;
            var headerCell = new Label(startColumn, startRow + i, header);
            var dataValueCell = new Label(startColumn + 1, startRow + i, dataValue);

            s.addCell(headerCell);
            s.addCell(dataValueCell);
        }
    }
}