using System;
using System.Collections.Generic;
using System.Linq;
using CSharpJExcel.Jxl.Write;

public class SimpleHorizontalSheetFiller : ISheetFiller
{
    int startRow;
    int startColumn;

    public SimpleHorizontalSheetFiller(int startRow, int startColumn)
    {
        if (startRow < 0)
        {
            throw new ArgumentOutOfRangeException("startRow");
        }

        if (startColumn < 0)
        {
            throw new ArgumentOutOfRangeException("startColumn");
        }

        this.startRow = startRow;
        this.startColumn = startColumn;
    }

    public void Fill(WritableSheet s, Dictionary<string, string> dataHeaderValues)
    {
        var dataHeaderValuesKeyPair = dataHeaderValues.ToArray();

        for (int i = 0; i < dataHeaderValuesKeyPair.Length; i++)
        {
            var dataHeaderValue = dataHeaderValuesKeyPair[i];
            var header = dataHeaderValue.Key;
            var dataValue = dataHeaderValue.Value;
            var headerCell = new Label(startColumn + i, startRow, header);
            var dataValueCell = new Label(startColumn + i, startRow + 1, dataValue);

            s.addCell(headerCell);
            s.addCell(dataValueCell);
        }
    }
}
