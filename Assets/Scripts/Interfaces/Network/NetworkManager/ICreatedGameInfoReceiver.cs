namespace Interfaces.Network.NetworkManager
{

    using System;

    using EventArgs;

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
