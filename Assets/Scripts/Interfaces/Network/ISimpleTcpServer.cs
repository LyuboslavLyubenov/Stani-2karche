namespace Assets.Scripts.Interfaces.Network
{

    using System;

    using Assets.Scripts.EventArgs;

    public interface ISimpleTcpServer : IDisposable
    {
        event EventHandler<IpEventArgs> OnClientConnected;

        event EventHandler<MessageEventArgs> OnReceivedMessage;
        
        int Port
        {
            get;
        }

        void Disconnect(string ipAddress, Action onSuccess = null, Action<Exception> onError = null);

        bool IsClientConnected(string ipAddress);
    }

}