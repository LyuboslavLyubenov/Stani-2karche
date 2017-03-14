namespace Assets.Tests.DummyObjects
{

    using System;
    using System.Collections.Generic;

    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Interfaces.Network;

    public class DummySimpleTcpServer : ISimpleTcpServer
    {        
        public event EventHandler<IpEventArgs> OnClientConnected = delegate {};

        public event EventHandler<MessageEventArgs> OnReceivedMessage = delegate {};
        
        public int Port { get; private set; }

        private HashSet<string> connectedClients = new HashSet<string>();
        
        public void Disconnect(string ipAddress, Action onSuccess = null, Action<Exception> onError = null)
        {
            if (this.connectedClients.Contains(ipAddress))
            {
                if (onSuccess != null)
                {
                    onSuccess();
                }
            }
            else
            {
                if (onError != null)
                {
                    onError(new Exception());
                }
            }
        }

        public bool IsClientConnected(string ipAddress)
        {
            return this.connectedClients.Add(ipAddress);
        }

        /// <summary>
        /// dont use me
        /// </summary>
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        private void ValidateIsConnected(string ipAddress)
        {
            if (!this.connectedClients.Contains(ipAddress))
            {
                throw new ArgumentException("Client not connected");
            }
        }

        public void FakeConnectClient(string ipAddress)
        {
            this.connectedClients.Add(ipAddress);
            this.OnClientConnected(this, new IpEventArgs(ipAddress));
        }

        public void FakeDisconnectClient(string ipAddress)
        {
            this.ValidateIsConnected(ipAddress);
            this.connectedClients.Remove(ipAddress);
        }

        public void FakeReceiveMessageFromClient(string ipAddress, string message)
        {
            this.ValidateIsConnected(ipAddress);
            this.OnReceivedMessage(this, new MessageEventArgs(ipAddress, message));
        }
    }

}