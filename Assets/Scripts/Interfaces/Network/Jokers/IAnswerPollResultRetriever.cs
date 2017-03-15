namespace Assets.Scripts.Interfaces.Network.Jokers
{
    using System;

    using Assets.Scripts.EventArgs;

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