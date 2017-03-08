﻿namespace Assets.Scripts.Interfaces.Network.Jokers.Routers
{

    using System;

    public interface IAskPlayerQuestionRouter : IJokerRouter, IDisposable
    {
        event EventHandler OnSent;
        
        bool Active { get; }

        void Activate(int senderConnectionId, int friendConnectionId, int timeToAnswerInSeconds);

        void Deactivate();
    }
}
