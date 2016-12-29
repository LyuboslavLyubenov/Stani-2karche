namespace Assets.Scripts.Network.TcpSockets
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;

    using Assets.Scripts.EventArgs;
    using Assets.Scripts.SecuritySettings;
    using Assets.Scripts.Utils;
    using Assets.Scripts.Utils.Unity;

    using Debug = UnityEngine.Debug;

    public class SimpleTcpServer : ExtendedMonoBehaviour
    {
        const int AcceptNewConnectionDelayInMiliseconds = 200;
        const float UpdateSocketsDelayInSeconds = 0.1f;
        protected const int ReceiveMessageTimeoutInMiliseconds = 1000;
        protected const int SendMessageTimeoutInMiliseconds = 1000;

        public EventHandler<IpEventArgs> OnClientConnected = delegate
            {
            };

        public EventHandler<MessageEventArgs> OnReceivedMessage = delegate
            {
            };

        //readonly object MyLock = new object();

        int port;
        bool initialized = false;

        readonly Socket acceptConnections = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        Dictionary<string, Socket> connectedIPClientsSocket = new Dictionary<string, Socket>();
        Dictionary<Socket, ReceiveMessageState> socketsMessageState = new Dictionary<Socket, ReceiveMessageState>();

        List<Socket> aliveSockets = new List<Socket>();

        public bool Initialized
        {
            get
            {
                return this.initialized;
            }
        }

        public int Port
        {
            get
            {
                return this.port;
            }
        }

        void OnDisable()
        {
            this.Dispose();
        }

        void OnApplicationExit()
        {
            this.Dispose();
        }

        void RemoveDisconnectedSockets()
        {
            var disconnectedSockets = this.connectedIPClientsSocket.Where(s => !s.Value.IsConnected()).ToList();

            disconnectedSockets.ForEach(ipSocket =>
                {
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
                });
        }

        string FilterReceivedMessage(string message)
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

        void BeginAcceptConnections()
        {
            this.acceptConnections.BeginAccept(new AsyncCallback(this.EndAcceptConnections), this.acceptConnections);
        }

        void EndAcceptConnections(IAsyncResult result)
        {
            var socket = (Socket)result.AsyncState;

            try
            {
                var connectionSocket = socket.EndAccept(result);
                var ip = (connectionSocket.RemoteEndPoint as IPEndPoint).Address.ToString().Split(':').First();

                connectionSocket.ReceiveTimeout = ReceiveMessageTimeoutInMiliseconds;
                connectionSocket.SendTimeout = SendMessageTimeoutInMiliseconds;

                if (this.connectedIPClientsSocket.ContainsKey(ip))
                {
                    this.Disconnect(ip);
                    Thread.Sleep(30);
                }

                this.connectedIPClientsSocket[ip] = connectionSocket;

                Debug.Log("SimpleTcpServer Accepted " + ip);

                this.BeginReceiveMessage(ip);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
            finally
            {
                Thread.Sleep(AcceptNewConnectionDelayInMiliseconds);
                this.BeginAcceptConnections();
            }
        }

        void BeginReceiveMessage(string ipAddress)
        {
            if (!ipAddress.IsValidIPV4())
            {
                throw new ArgumentException("Invalid ipv4 address");
            }

            if (!this.connectedIPClientsSocket.ContainsKey(ipAddress))
            {
                throw new Exception("Not connected to " + ipAddress);
            }

            var socket = this.connectedIPClientsSocket[ipAddress];
            var state = new ReceiveMessageState(socket);

            if (!this.socketsMessageState.ContainsKey(socket))
            {
                this.socketsMessageState.Add(socket, state);
            }

            socket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, new AsyncCallback(this.EndReceiveMessage), state);
    
            Debug.Log("SimpleTcpServer BeginReceiveMessage from " + ipAddress);
        }

        void EndReceiveMessage(IAsyncResult result)
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
                    Debug.Log(ex.Message);   
                }

                return;
            }

            if (!state.IsReceivedDataSize && bytesReceivedCount >= 4)
            {
                state.DataSizeNeeded = BitConverter.ToInt32(state.Buffer, 0);
                state.IsReceivedDataSize = true;
                offset += 4;
                bytesReceivedCount -= 4;
            }

            state.Data.Write(state.Buffer, offset, bytesReceivedCount);

            if (state.Data.Length == state.DataSizeNeeded)
            {
                var buffer = state.Data.ToArray();
                var message = Encoding.UTF8.GetString(buffer);
                var filteredMessage = this.FilterReceivedMessage(message);
                var decryptedMessage = CipherUtility.Decrypt<RijndaelManaged>(filteredMessage, SecuritySettings.NETWORK_ENCRYPTION_PASSWORD, SecuritySettings.SALT);
                var args = new MessageEventArgs(state.IPAddress, decryptedMessage);

                Debug.Log("SimpleTcpServer Received " + decryptedMessage + " from " + state.IPAddress);

                if (this.OnReceivedMessage != null)
                {
                    ThreadUtils.Instance.RunOnMainThread(() => this.OnReceivedMessage(this, args));
                }

                this.socketsMessageState[socket] = new ReceiveMessageState(socket);
            }

            Thread.Sleep(30);
            socket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, new AsyncCallback(this.EndReceiveMessage), state);
        }

        public void Initialize(int port)
        {
            if (this.Initialized)
            {
                throw new InvalidOperationException("Already initialized");
            }

            var threadUtils = ThreadUtils.Instance;//initialize

            this.port = port;
            this.acceptConnections.ExclusiveAddressUse = false;
            this.acceptConnections.SendTimeout = SendMessageTimeoutInMiliseconds;
            this.acceptConnections.ReceiveTimeout = ReceiveMessageTimeoutInMiliseconds;
            this.acceptConnections.Bind(new IPEndPoint(IPAddress.Any, this.Port));
            this.acceptConnections.Listen(10);

            this.CoroutineUtils.RepeatEverySeconds(1f, this.RemoveDisconnectedSockets);

            this.BeginAcceptConnections();

            this.initialized = true;

            Debug.Log("SimpleTcpServer initialized");
        }

        public void Disconnect(string ipAddress)
        {
            if (!this.initialized)
            {
                throw new Exception("Use initialize method first");
            }

            if (!this.connectedIPClientsSocket.ContainsKey(ipAddress))
            {
                throw new ArgumentException("Not connected to " + ipAddress);
            }

            var socket = this.connectedIPClientsSocket[ipAddress];
            var state = new DisconnectState(ipAddress, socket);

            socket.BeginDisconnect(false, new AsyncCallback(this.EndDisconnect), state);

            Debug.Log("SimpleTcpServer begin disconnect " + ipAddress);
        }

        void EndDisconnect(IAsyncResult result)
        {
            var state = (DisconnectState)result.AsyncState;
            var socket = state.Socket;

            try
            {
                this.connectedIPClientsSocket.Remove(state.IPAddress);
                socket.EndDisconnect(result);
                socket.Close();
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }

            Debug.Log("SimpleTcpServer disconnected " + state.IPAddress);
        }

        public virtual void Dispose()
        {
            try
            {
                this.acceptConnections.Close();    
            }
            catch
            {
            
            }
       
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

            this.connectedIPClientsSocket.Clear();
            this.socketsMessageState.Clear();

            this.initialized = false;

            Debug.Log("SimpleTcpServer disposed");
        }

        public bool IsClientConnected(string ipAddress)
        {
            return this.connectedIPClientsSocket.ContainsKey(ipAddress);
        }
        //*/
    }

}
