namespace Assets.Scripts.Interfaces.Network
{

    using System;

    public interface ISimpleTcpClient : IDisposable
    {
        void Send(string ipAddress, string message, Action onSent = null, Action<Exception> onError = null);

        void ConnectTo(string ipAddress, int port, Action onConnected = null, Action<Exception> onError = null);

        void DisconnectFrom(string ipAddress, Action onSuccess = null, Action<Exception> onError = null);

        bool IsConnectedTo(string ipAddress);
    }

}