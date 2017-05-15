namespace Assets.Scripts.EventArgs
{

    using System;

    public class ChangedAnswersCountEventArgs : EventArgs
    {
        public int Count
        {
            get; private set;
        }

        public ChangedAnswersCountEventArgs(int count)
        {
            this.Count = count;
        }
    }

}