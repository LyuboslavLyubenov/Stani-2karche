namespace Assets.Scripts.Network.TcpSockets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Security.Cryptography;
    using System.Text;
    using System.Timers;

    using Assets.Scripts.Extensions;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Network;

    using SecuritySettings;
    using Utils;

    using Debug = UnityEngine.Debug;

    public class SimpleTcpClient : ISimpleTcpClient
    {
        private const int ReceiveMessageTimeoutInMiliseconds = 4000;
        private const int SendMessageTimeoutInMiliseconds = 4000;

        private Dictionary<string, Socket> connectedToServersIPsSockets = new Dictionary<string, Socket>();

        private readonly object myLock = new object();

        private Timer updateConnectedSocketsTimer;

        public SimpleTcpClient()
        {
            var threadUtils = ThreadUtils.Instance;//initialize

            this.updateConnectedSocketsTimer = TimerUtils.ExecuteEvery(1f, this.UpdateConnectedSockets);
            this.updateConnectedSocketsTimer.Start();
        }

        private void UpdateConnectedSockets()
        {
            lock (this.myLock)
            {
                this.connectedToServersIPsSockets.Where(ipSocket => !ipSocket.Value.IsConnected())
                .ToList()
                .ForEach(ipSocket =>
                {
                    try
                    {
                        ipSocket.Value.Close();
                    }
                    catch (Exception ex)
                    {
                        Debug.Log(ex.Message);
                    }

                    this.connectedToServersIPsSockets.Remove(ipSocket.Key);
                });
            }
        }

        private void BeginConnectToServer(string ipAddress, int port, Action onConnected = null, Action<Exception> onError = null)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var state = new ClientConnectingState() { OnSuccess = onConnected, OnError = onError, Client = socket };

            socket.ExclusiveAddressUse = false;

            socket.SendTimeout = ReceiveMessageTimeoutInMiliseconds;
            socket.ReceiveTimeout = SendMessageTimeoutInMiliseconds;

            socket.BeginConnect(ipAddress, port, new AsyncCallback(this.EndConnectToServer), state);
        }

        private void EndConnectToServer(IAsyncResult result)
        {
            var state = (ClientConnectingState)result.AsyncState;
            var socket = state.Client;

            try
            {
                socket.EndConnect(result);

                var serverIpEndPoint = (IPEndPoint)socket.RemoteEndPoint;
                var ipAddress = serverIpEndPoint.Address.ToString().Split(':').First();

                lock (this.myLock)
                {
                    this.connectedToServersIPsSockets.Add(ipAddress, socket);
                }

                if (state.OnSuccess != null)
                {
                    ThreadUtils.Instance.RunOnMainThread(state.OnSuccess);
                }
            }
            catch (Exception exception)
            {
                if (state.OnError != null)
                {
                    ThreadUtils.Instance.RunOnMainThread(() => state.OnError(exception));
                }
            }
        }

        private void BeginSendMessageToServer(string ipAddress, string message, Action onSent = null, Action<Exception> onError = null)
        {
            Socket socket;

            lock (myLock)
            {
                socket = this.connectedToServersIPsSockets[ipAddress];
            }

            var encryptedMessage = CipherUtility.Encrypt<RijndaelManaged>(message, SecuritySettings.NETWORK_ENCRYPTION_PASSWORD, SecuritySettings.SALT);
            var messageBuffer = Encoding.UTF8.GetBytes(encryptedMessage);
            var prefix = BitConverter.GetBytes(messageBuffer.Length);
            var state = new SendMessageState() { Client = socket, DataToSend = new byte[messageBuffer.Length + 4], OnSuccess = onSent, OnError = onError };

            Buffer.BlockCopy(prefix, 0, state.DataToSend, 0, prefix.Length);
            Buffer.BlockCopy(messageBuffer, 0, state.DataToSend, prefix.Length, messageBuffer.Length);

            socket.BeginSend(state.DataToSend, 0, state.DataToSend.Length, SocketFlags.None, new AsyncCallback(this.EndSendMessageToServer), state);
        }

        private void EndSendMessageToServer(IAsyncResult result)
        {
            var state = (SendMessageState)result.AsyncState;
            var socket = state.Client;

            try
            {
                var sendBytes = socket.EndSend(result);
                state.DataSentLength += sendBytes;

                if (state.DataSentLength < state.DataToSend.Length)
                {
                    var sendSize = state.DataToSend.Length - state.DataSentLength;
                    socket.BeginSend(state.DataToSend, state.DataSentLength, sendSize, SocketFlags.None, new AsyncCallback(this.EndSendMessageToServer), state);
                }
                else
                {
                    if (state.OnSuccess != null)
                    {
                        ThreadUtils.Instance.RunOnMainThread(state.OnSuccess);
                    }
                }
            }
            catch (Exception exception)
            {
                if (state.OnError != null)
                {
                    ThreadUtils.Instance.RunOnMainThread(() => state.OnError(exception));
                }
            }
        }

        private void EndDisconnect(IAsyncResult result)
        {
            var state = (DisconnectState)result.AsyncState;
            var socket = state.Socket;

            if (!socket.Connected)
            {
                return;
            }

            try
            {
                lock (myLock)
                {
                    this.connectedToServersIPsSockets.Remove(state.IPAddress);
                }

                socket.Close();

                if (state.OnSuccess != null)
                {
                    ThreadUtils.Instance.RunOnMainThread(state.OnSuccess);
                }
            }
            catch (Exception exception)
            {
                if (state.OnError != null)
                {
                    ThreadUtils.Instance.RunOnMainThread(() => state.OnError(exception));
                }
            }
        }

        public void Send(string ipAddress, string message, Action onSent = null, Action<Exception> onError = null)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException("message", "Cannot send empty message");
            }

            lock (myLock)
            {
                if (!this.connectedToServersIPsSockets.ContainsKey(ipAddress))
                {
                    throw new Exception("Not connected to " + ipAddress);
                }
            }

            this.BeginSendMessageToServer(ipAddress, message, onSent, onError);
        }

        public void ConnectTo(string ipAddress, int port, Action onConnected = null, Action<Exception> onError = null)
        {
            if (!ipAddress.IsValidIPV4())
            {
                throw new ArgumentException("Invalid Ipv4 address");
            }

            if (port < 0)
            {
                throw new ArgumentOutOfRangeException("port", "Invalid port");
            }

            lock (myLock)
            {
                if (this.connectedToServersIPsSockets.ContainsKey(ipAddress))
                {
                    throw new Exception("Already connected to " + ipAddress);
                }
            }

            this.BeginConnectToServer(ipAddress, port, onConnected, onError);
        }

        public void DisconnectFrom(string ipAddress, Action onSuccess = null, Action<Exception> onError = null)
        {
            Socket socket;

            lock (myLock)
            {

                if (!this.connectedToServersIPsSockets.ContainsKey(ipAddress))
                {
                    throw new Exception("Not connected to " + ipAddress);
                }

                socket = this.connectedToServersIPsSockets[ipAddress];
            }

            var state = new DisconnectState()
                        {
                            IPAddress = ipAddress,
                            OnSuccess = onSuccess,
                            OnError = onError,
                            Socket = socket
                        };

            socket.BeginDisconnect(false, this.EndDisconnect, state);
        }

        public void Dispose()
        {
            lock (this.myLock)
            {
                this.connectedToServersIPsSockets.ToList()
                    .ForEach(ipSocket =>
                    {
                        try
                        {
                            ipSocket.Value.Close();
                        }
                        catch
                        {
                        }
                    });

                this.connectedToServersIPsSockets.Clear();
            }

            this.updateConnectedSocketsTimer.Stop();
            this.updateConnectedSocketsTimer.Dispose();
            this.updateConnectedSocketsTimer = null;
        }

        public bool IsConnectedTo(string ipAddress)
        {
            bool isConnected = false;

            lock (this.myLock)
            {
                isConnected = this.connectedToServersIPsSockets.ContainsKey(ipAddress);
            }

            return isConnected;
        }
    }
}