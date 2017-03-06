namespace Assets.Scripts.Interfaces.Network.Jokers
{

    using System;

    using Assets.Scripts.DTOs;

    public interface IAudienceAnswerPollRouter : IJokerRouter, IDisposable
    {
        event EventHandler OnBeforeSend;

        event EventHandler OnSent;

        bool Activated { get; }

        void Activate(int senderConnectionId, MainPlayerData mainPlayerData, int timeToAnswerInSeconds);

        void Deactivate();
    }
}
