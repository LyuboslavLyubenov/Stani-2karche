using System;
using System.Collections.Generic;
using System.IO;
using CSharpJExcel.Jxl;
using UnityEngine;
using System.Linq;
using CSharpJExcel.Jxl.Write;
using CSharpJExcel.Jxl.Biff;

public interface ISheetFiller
{
    void Fill(WritableSheet s, Dictionary<string, string> dataHeaderValues);
}
