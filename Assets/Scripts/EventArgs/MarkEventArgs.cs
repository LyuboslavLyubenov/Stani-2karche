using System;

namespace Assets.Scripts.EventArgs
{

    using EventArgs = System.EventArgs;

    [Serializable]
    public sealed class MarkEventArgs : EventArgs
    {
        public MarkEventArgs(int mark)
        {
            if (mark < 2 || mark > 6)
            {
                throw new ArgumentOutOfRangeException("mark", "Оценката трябва да е между 2 и 6 (включително)");
            }

            this.Mark = mark;
        }

        public int Mark
        {
            get;
            private set;
        }
    }

}