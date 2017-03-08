namespace Assets.Tests
{

    using System;
    using System.Collections.Generic;

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Network;

    public class DummySimpleTcpClient : ISimpleTcpClient
    {
        private readonly HashSet<string> connectedServersIps = new HashSet<string>();

        public void Send(string ipAddress, string message, Action onSent = null, Action<Exception> onError = null)
        {
            if (!this.connectedServersIps.Contains(ipAddress))
            {
                if (onError != null)
                {
                    onError(new ArgumentException("Not connected to server"));
                }
            }
            else
            {
                if (onSent != null)
                {
                    onSent();
                }
            }
        }

        public void ConnectTo(string ipAddress, int port, Action onConnected = null, Action<Exception> onError = null)
        {
            if (this.connectedServersIps.Contains(ipAddress))
            {
                if (onError != null)
                {
                    onError(new ArgumentException("Already connected"));
                }
            }
            else
            {
                this.connectedServersIps.Add(ipAddress);

                if (onConnected != null)
                {
                    onConnected();
                }
            }
        }

        public void DisconnectFrom(string ipAddress, Action onSuccess = null, Action<Exception> onError = null)
        {
            if (!this.connectedServersIps.Contains(ipAddress))
            {
                if (onError != null)
                {
                    onError(new ArgumentException("Not connected to " + ipAddress));
                }
            }
            else
            {
                this.connectedServersIps.Remove(ipAddress);

                if (onSuccess != null)
                {
                    onSuccess();
                }
            }
        }

        public bool IsConnectedTo(string ipAddress)
        {
            return this.connectedServersIps.Contains(ipAddress);
        }
        
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

}