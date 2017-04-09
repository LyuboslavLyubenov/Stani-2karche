namespace EventArgs.Jokers
{

    using System;

    public class ElectionJokerResultEventArgs : EventArgs
    {
        public ElectionDecision ElectionDecision
        {
            get; private set;
        }

        public ElectionJokerResultEventArgs(ElectionDecision electionDecision)
        {
            this.ElectionDecision = electionDecision;
        }
    }

    public enum ElectionDecision
    {
        For,
        Against
    }
}
