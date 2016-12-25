using System;

namespace Assets.Scripts.Statistics
{

    public class JExcelCellPosition
    {
        public int Column
        {
            get;
            private set;
        }

        public int Row
        {
            get;
            private set;
        }

        public string Address
        {
            get
            {
                var reference = CSharpJExcel.Jxl.Biff.CellReferenceHelper.getCellReference(this.Column, this.Row);
                return reference;
            }
        }

        public JExcelCellPosition(int column, int row)
        {
            if (column < 0)
            {
                throw new ArgumentOutOfRangeException("column");
            }

            if (row < 0)
            {
                throw new ArgumentOutOfRangeException("row");
            }

            this.Column = column;
            this.Row = row;
        }
    }

}
