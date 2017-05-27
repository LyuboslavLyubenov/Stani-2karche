namespace Assets.Tests.DummyObjects.Network
{
    using System;
    using System.Collections.Generic;

    using EventArgs;

    using Interfaces.Network.Jokers;
    
    public class DummyAnswerPollResultRetriever : IAnswerPollResultRetriever
    {
        public event EventHandler OnReceivedVoteTimeout = delegate {};
        public event EventHandler OnReceivedSettingsTimeout = delegate {};
        public event EventHandler<JokerSettingsEventArgs> OnReceivedSettings = delegate {};
        public event EventHandler<VoteEventArgs> OnVoted = delegate {};

        public bool Activated { get; private set; }

        public void SimulateRecevedVoteTimeout()
        {
            this.OnReceivedVoteTimeout(this, EventArgs.Empty);
        }

        public void SimulateReceivedSettingsTimeout()
        {
            this.OnReceivedSettingsTimeout(this, EventArgs.Empty);
        }

        public void SimulateReceivedSettings(int timeToAnswerInSeconds)
        {
            this.OnReceivedSettings(this, new JokerSettingsEventArgs(timeToAnswerInSeconds));
        }

        public void SimulateOnVoted(Dictionary<string, int> answersVotes)
        {
            this.OnVoted(this, new VoteEventArgs(answersVotes));
        }

        public void Activate()
        {
            this.Activated = true;
        }
        
        public void Dispose()
        {
        }
    }
}