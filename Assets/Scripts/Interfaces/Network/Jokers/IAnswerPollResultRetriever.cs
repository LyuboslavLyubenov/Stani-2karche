namespace Interfaces.Network.Jokers
{

    using System;

    using EventArgs;

    public interface IAnswerPollResultRetriever : IDisposable
    {
        event EventHandler OnReceivedVoteTimeout;

        event EventHandler OnReceivedSettingsTimeout;

        event EventHandler<JokerSettingsEventArgs> OnReceivedSettings;

        event EventHandler<VoteEventArgs> OnVoted;

        bool Activated { get; }

        void Activate();
    }
}