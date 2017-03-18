namespace Statistics
{

    using System.IO;

    using CSharpJExcel.Jxl;
    using CSharpJExcel.Jxl.Write;

    public class WorkbookUtils
    {
        private WorkbookUtils()
        {
        
        }

        public static WritableWorkbook CreateEmptyWorkbook(FileInfo fileInfo, string sheetName = "Sheet1")
        {
            var workbook = Workbook.createWorkbook(fileInfo);
            workbook.createSheet(sheetName, 0).addCell(new Label(0, 0, ""));
            workbook.write();
            return workbook;
        }
    }

}


