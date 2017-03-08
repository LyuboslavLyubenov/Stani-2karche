﻿namespace Assets.Scripts.Interfaces.Network.NetworkManager
{

    using System;

    using Assets.Scripts.EventArgs;

    public interface ICreatedGameInfoReceiver
    {
        void ReceiveFrom(
            string ipAddress,
            Action<GameInfoReceivedDataEventArgs> receivedGameInfo,
            Action<Exception> onError = null);

        void StopReceivingFrom(string ipAddress);

        void StopReceivingFromAll();
    }
}
