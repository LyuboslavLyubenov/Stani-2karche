namespace Assets.Scripts.Interfaces
{

    using System;

    using Assets.Scripts.EventArgs;

    public interface ISimpleTcpServer : IDisposable
    {
        event EventHandler<IpEventArgs> OnClientConnected;

        event EventHandler<MessageEventArgs> OnReceivedMessage;

        event EventHandler<IpEventArgs> OnClientDisconnected;

        int Port
        {
            get;
        }

        void Disconnect(string ipAddress, Action onSuccess = null, Action<Exception> onError = null);

        bool IsClientConnected(string ipAddress);
    }

}