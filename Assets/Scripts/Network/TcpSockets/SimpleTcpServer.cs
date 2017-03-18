namespace Network.TcpSockets
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;

    using EventArgs;

    using Extensions;

    using Interfaces.Network;

    using SecuritySettings;

    using Utils;

    using Debug = UnityEngine.Debug;
    using Timer = System.Timers.Timer;

    public class SimpleTcpServer : ISimpleTcpServer
    {
        private const int MessageBuffer = 1024;

        private const int ReceiveMessageTimeoutInMiliseconds = 4000;
        private const int SendMessageTimeoutInMiliseconds = 4000;

        private const int AcceptNewConnectionDelayInMiliseconds = 200;

        private const float UpdateSocketsDelayInSeconds = 0.1f;
        private const float CheckDisconnectedSocketsDelayInSeconds = 1f;

        public event EventHandler<IpEventArgs> OnClientConnected = delegate
            {
            };

        public event EventHandler<MessageEventArgs> OnReceivedMessage = delegate
            {
            };
        
        readonly object myLock = new object();

        public int Port
        {
            get;
            private set;
        }

        private readonly Socket acceptConnections = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private Dictionary<string, Socket> connectedIPClientsSocket = new Dictionary<string, Socket>();
        private Dictionary<Socket, ReceiveMessageState> socketsMessageState = new Dictionary<Socket, ReceiveMessageState>();

        private Timer removeDisconnectedSocketsTimer;

        public SimpleTcpServer(int port)
        {
            var threadUtils = ThreadUtils.Instance;//initialize

            this.Port = port;
            this.acceptConnections.ExclusiveAddressUse = false;
            this.acceptConnections.SendTimeout = SendMessageTimeoutInMiliseconds;
            this.acceptConnections.ReceiveTimeout = ReceiveMessageTimeoutInMiliseconds;
            this.acceptConnections.Bind(new IPEndPoint(IPAddress.Any, this.Port));
            this.acceptConnections.Listen(1);

            this.removeDisconnectedSocketsTimer = TimerUtils.ExecuteEvery(CheckDisconnectedSocketsDelayInSeconds, this.RemoveDisconnectedSockets);
            this.removeDisconnectedSocketsTimer.Start();

            this.BeginAcceptConnections();
        }

        private void RemoveDisconnectedSockets()
        {
            lock (this.myLock)
            {
                var disconnectedSockets = this.connectedIPClientsSocket.Where(s => !s.Value.IsConnected()).ToList();

                for (int i = 0; i < disconnectedSockets.Count; i++)
                {
                    var ipSocket = disconnectedSockets[i];

                    try
                    {
                        ipSocket.Value.Close();
                        this.socketsMessageState.Remove(ipSocket.Value);
                    }
                    catch (Exception ex)
                    {
                        Debug.Log(ex.Message);
                    }

                    this.connectedIPClientsSocket.Remove(ipSocket.Key);
                }
            }
        }

        private string FilterReceivedMessage(string message)
        {
            var filtered = new StringBuilder();

            for (int i = 0; i < message.Length; i++)
            {
                if (message[i] != '\0')
                {
                    filtered.Append(message[i]);
                }
            }

            return filtered.ToString();
        }

        private void BeginAcceptConnections()
        {
            this.acceptConnections.BeginAccept(new AsyncCallback(this.EndAcceptConnections), this.acceptConnections);
        }

        private void EndAcceptConnections(IAsyncResult result)
        {
            var socket = (Socket)result.AsyncState;

            try
            {
                var connectionSocket = socket.EndAccept(result);
                var ip = (connectionSocket.RemoteEndPoint as IPEndPoint).Address.ToString().Split(':').First();

                connectionSocket.ReceiveTimeout = ReceiveMessageTimeoutInMiliseconds;
                connectionSocket.SendTimeout = SendMessageTimeoutInMiliseconds;

                lock (this.myLock)
                {
                    var isConnectedAlready = this.connectedIPClientsSocket.ContainsKey(ip);

                    if (isConnectedAlready)
                    {
                        this.Disconnect(ip);
                        Thread.Sleep(30);
                    }

                    this.connectedIPClientsSocket[ip] = connectionSocket;
                }

                ThreadUtils.Instance.RunOnMainThread(() => this.OnClientConnected(this, new IpEventArgs(ip)));

                this.BeginReceiveMessage(ip);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
            }

            Thread.Sleep(AcceptNewConnectionDelayInMiliseconds);
            this.BeginAcceptConnections();
        }

        private void BeginReceiveMessage(string ipAddress)
        {
            lock (this.myLock)
            {
                var socket = this.connectedIPClientsSocket[ipAddress];
                var state = new ReceiveMessageState(socket, MessageBuffer);

                if (!this.socketsMessageState.ContainsKey(socket))
                {
                    this.socketsMessageState.Add(socket, state);
                }

                socket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, new AsyncCallback(this.EndReceiveMessage), state);
            }
        }

        private void EndReceiveMessage(IAsyncResult result)
        {
            var state = (ReceiveMessageState)result.AsyncState;
            var socket = state.Socket;
            var offset = 0;

            SocketError socketState;
            var bytesReceivedCount = socket.EndReceive(result, out socketState);

            if (bytesReceivedCount == 0 || socketState != SocketError.Success)
            {
                try
                {
                    socket.Close();
                }
                catch (Exception ex)
                {
                    Debug.LogWarning(ex.Message);
                }

                return;
            }

            if (!state.IsReceivedDataSize && bytesReceivedCount >= 4)
            {
                state.DataSizeNeeded = BitConverter.ToInt32(state.Buffer, 0);
                offset += 4;
                bytesReceivedCount -= 4;
            }

            state.Data.Write(state.Buffer, offset, bytesReceivedCount);

            if (state.IsReceivedDataSize && state.Data.Length == state.DataSizeNeeded)
            {
                var buffer = state.Data.ToArray();
                var message = Encoding.UTF8.GetString(buffer);
                var filteredMessage = this.FilterReceivedMessage(message);
                var decryptedMessage = CipherUtility.Decrypt<RijndaelManaged>(filteredMessage, SecuritySettings.NETWORK_ENCRYPTION_PASSWORD, SecuritySettings.SALT);
                var args = new MessageEventArgs(state.IPAddress, decryptedMessage);

                if (this.OnReceivedMessage != null)
                {
                    ThreadUtils.Instance.RunOnMainThread(() => this.OnReceivedMessage(this, args));
                }

                lock (this.myLock)
                {
                    this.socketsMessageState[socket] = new ReceiveMessageState(socket, MessageBuffer);
                }
            }

            Thread.Sleep(30);
            socket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, new AsyncCallback(this.EndReceiveMessage), state);
        }

        private void EndDisconnect(IAsyncResult result)
        {
            var state = (DisconnectState)result.AsyncState;
            var socket = state.Socket;

            try
            {
                lock (this.myLock)
                {
                    this.connectedIPClientsSocket.Remove(state.IPAddress);

                    if (this.socketsMessageState.ContainsKey(socket))
                    {
                        this.socketsMessageState.Remove(socket);
                    }
                }

                socket.EndDisconnect(result);

                if (state.OnSuccess != null)
                {
                    state.OnSuccess();
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);

                if (state.OnError != null)
                {
                    state.OnError(ex);
                }
            }
        }

        private void DisconnectAllSockets()
        {
            this.connectedIPClientsSocket.ToList().ForEach(ipSocket =>
            {
                try
                {
                    ipSocket.Value.Close();
                }
                catch
                {
                }
            });

            this.connectedIPClientsSocket.Clear();
        }

        private void ClearMessagesRequestQueue()
        {
            this.socketsMessageState.ToList().ForEach(socketState =>
            {
                try
                {
                    socketState.Key.Close();
                }
                catch
                {
                }
            });

            this.socketsMessageState.Clear();
        }

        public void Disconnect(string ipAddress, Action onSuccess = null, Action<Exception> onError = null)
        {
            lock (this.myLock)
            {
                if (!this.connectedIPClientsSocket.ContainsKey(ipAddress))
                {
                    throw new ArgumentException("Not connected to " + ipAddress);
                }

                var socket = this.connectedIPClientsSocket[ipAddress];
                var state = new DisconnectState() { IPAddress = ipAddress, OnSuccess = onSuccess, OnError = onError, Socket = socket };

                socket.BeginDisconnect(false, new AsyncCallback(this.EndDisconnect), state);
            }
        }

        public void Dispose()
        {
            try
            {
                this.acceptConnections.Close();
            }
            catch
            {
            }
            
            this.DisconnectAllSockets();
            this.ClearMessagesRequestQueue();
            
            this.removeDisconnectedSocketsTimer.Stop();
            this.removeDisconnectedSocketsTimer.Dispose();
            this.removeDisconnectedSocketsTimer = null;
        }

        public bool IsClientConnected(string ipAddress)
        {
            return this.connectedIPClientsSocket.ContainsKey(ipAddress);
        }
    }
}