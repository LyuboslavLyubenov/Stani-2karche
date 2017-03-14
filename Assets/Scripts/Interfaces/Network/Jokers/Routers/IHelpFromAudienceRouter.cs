namespace Assets.Scripts.Interfaces.Network.Jokers.Routers
{

    using System;

    using Assets.Scripts.DTOs;

    /// <summary>
    /// sender = senderConnectionId
    /// Uses AnswerPoll and sends it to the sender.
    /// If audience is less than 4 people, fake voting (generate/simulate) and send it to the sender.
    /// </summary>
    public interface IHelpFromAudienceJokerRouter : IJokerRouter, IDisposable
    {
        event EventHandler OnBeforeSend;

        event EventHandler OnSent;

        bool Activated { get; }

        void Activate(int senderConnectionId, int timeToAnswerInSeconds);

        void Deactivate();
    }
}
