namespace EventArgs
{

    using System;

    using EventArgs = System.EventArgs;

    [Serializable]
    public sealed class MarkEventArgs : EventArgs
    {
        public const int MinMark = 2;
        public const int MaxMark = 6;

        public MarkEventArgs(int mark)
        {
            if (mark < MinMark || mark > MaxMark)
            {
                var exceptionMessage = string.Format("Оценката трябва да е между {0} и {1} (включително)", MinMark, MaxMark);
                throw new ArgumentOutOfRangeException("mark", exceptionMessage);
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