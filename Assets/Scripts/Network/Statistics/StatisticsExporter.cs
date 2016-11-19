using System;
using System.Collections.Generic;
using System.IO;
using CSharpJExcel.Jxl;
using UnityEngine;
using System.Linq;
using CSharpJExcel.Jxl.Write;
using CSharpJExcel.Jxl.Biff;

public class StatisticsExporter
{
    public EventHandler OnBeforeSave = delegate
    {
    };

    public EventHandler OnSaved = delegate
    {
    };

    FileInfo fileInfo;

    Dictionary<string, string> dataHeaderValues = new Dictionary<string, string>();

    public StatisticsExporter(string path)
    {
        fileInfo = new FileInfo(path);
    }

    public void AddData(string header, string data)
    {
        if (header == null)
        {
            throw new ArgumentNullException("header");
        }
            
        if (data == null)
        {
            throw new ArgumentNullException("data");
        }
            
        if (dataHeaderValues.ContainsKey("header"))
        {
            throw new ArgumentException("Header already exists");
        }

        dataHeaderValues.Add(header, data);
    }

    public void Export(ISheetFiller sheetFiller)
    {
        OnBeforeSave(this, EventArgs.Empty);

        if (fileInfo.Exists)
        {
            fileInfo.Delete();
        }

        var workbookW = Workbook.createWorkbook(fileInfo);
        var sheetW = workbookW.createSheet("General", 0);
        var data = dataHeaderValues.ToArray();

        sheetFiller.Fill(sheetW, dataHeaderValues);

        workbookW.write();
        workbookW.close();

        OnSaved(this, EventArgs.Empty);
    }
}
