namespace Statistics
{

    using CSharpJExcel.Jxl;
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
            catch
            {
                cell = new EmptyCell(column, row);
            }

            return cell;
        }

        public static bool IsCellEmpty(this Sheet sheet, int column, int row)
        {
            var empty = false;

            try
            {
                sheet.getCell(column, row);
            }
            catch
            {
                empty = true;
            }

            return empty;
        }
    }

}