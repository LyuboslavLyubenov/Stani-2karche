namespace Assets.Scripts.Interfaces
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
