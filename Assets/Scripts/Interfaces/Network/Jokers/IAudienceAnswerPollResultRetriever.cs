namespace Assets.Scripts.Interfaces.Network.Jokers
{

    using System;

    using Assets.Scripts.EventArgs;

    public interface IAudienceAnswerPollResultRetriever : IDisposable
    {
        event EventHandler<AudienceVoteEventArgs> OnAudienceVoted;

        event EventHandler<JokerSettingsEventArgs> OnReceivedSettings;

        event EventHandler OnReceiveSettingsTimeout;

        event EventHandler OnReceiveAudienceVoteTimeout;

        event EventHandler OnActivated;

        bool Activated
        {
            get;
        }

        void Activate();
    }

}