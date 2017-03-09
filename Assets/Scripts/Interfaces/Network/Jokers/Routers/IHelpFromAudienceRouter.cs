namespace Assets.Scripts.Interfaces.Network.Jokers.Routers
{

    using System;

    using Assets.Scripts.DTOs;

    public interface IHelpFromAudienceJokerRouter : IJokerRouter, IDisposable
    {
        event EventHandler OnBeforeSend;

        event EventHandler OnSent;

        bool Activated { get; }

        void Activate(int senderConnectionId, int timeToAnswerInSeconds);

        void Deactivate();
    }
}
