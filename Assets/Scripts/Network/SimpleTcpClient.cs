using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace Assets.Scripts.Network
{

    using Assets.Scripts.SecuritySettings;
    using Assets.Scripts.Utils;

    using Debug = UnityEngine.Debug;

    public class SimpleTcpClient : ExtendedMonoBehaviour
    {
        protected const int ReceiveMessageTimeoutInMiliseconds = 1000;
        protected const int SendMessageTimeoutInMiliseconds = 1000;

        Dictionary<string, Socket> connectedToServersIPsSockets = new Dictionary<string, Socket>();

        //readonly object MyLock = new object();

        bool initialized = false;

        public bool Initialized
        {
            get
            {
                return this.initialized;
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

        void UpdateConnectedSockets()
        {
            var disconnectedSockets = this.connectedToServersIPsSockets.Where(ipSocket => !ipSocket.Value.IsConnected()).ToList();
            disconnectedSockets.ForEach(ipSocket =>
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

        void BeginConnectToServer(string ipAddress, int port, Action OnConnected)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var state = new ClientConnectingState() { OnConnected = OnConnected, Client = socket };

            socket.ExclusiveAddressUse = false;

            socket.SendTimeout = ReceiveMessageTimeoutInMiliseconds;
            socket.ReceiveTimeout = SendMessageTimeoutInMiliseconds;

            socket.BeginConnect(ipAddress, port, new AsyncCallback(this.EndConnectToServer), state);
        }

        void EndConnectToServer(IAsyncResult result)
        {
            var state = (ClientConnectingState)result.AsyncState;
            var socket = state.Client;

            try
            {
                socket.EndConnect(result);

                var serverIpEndPoint = (IPEndPoint)socket.RemoteEndPoint;
                var ipAddress = serverIpEndPoint.Address.ToString().Split(':').First();

                this.connectedToServersIPsSockets.Add(ipAddress, socket);

                ThreadUtils.Instance.RunOnMainThread(state.OnConnected);

                Debug.Log("SimpleTcpClient Connected to " + ipAddress);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);   
            }
        }

        void BeginSendMessageToServer(string ipAddress, string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException("message", "Cannot send empty message");
            }

            if (!this.connectedToServersIPsSockets.ContainsKey(ipAddress))
            {
                throw new Exception("Not connected to " + ipAddress);
            }

            var socket = this.connectedToServersIPsSockets[ipAddress];
            var encryptedMessage = CipherUtility.Encrypt<RijndaelManaged>(message, SecuritySettings.NETWORK_ENCRYPTION_PASSWORD, SecuritySettings.SALT);
            var messageBuffer = Encoding.UTF8.GetBytes(encryptedMessage);
            var prefix = BitConverter.GetBytes(messageBuffer.Length);
            var state = new SendMessageState() { Client = socket, DataToSend = new byte[messageBuffer.Length + 4] };

            Buffer.BlockCopy(prefix, 0, state.DataToSend, 0, prefix.Length);
            Buffer.BlockCopy(messageBuffer, 0, state.DataToSend, prefix.Length, messageBuffer.Length);

            socket.BeginSend(state.DataToSend, 0, state.DataToSend.Length, SocketFlags.None, new AsyncCallback(this.EndSendMessageToServer), state);
        }

        void EndSendMessageToServer(IAsyncResult result)
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
                    Debug.Log("SimpleTcpClient Sent " + Encoding.UTF8.GetString(state.DataToSend));
                    //TODO: ON SENT MESSAGE EVENT
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);    
            }
        }

        public void Send(string ipAddress, string message)
        {
            if (!this.connectedToServersIPsSockets.ContainsKey(ipAddress))
            {
                throw new Exception("Not connected to " + ipAddress);
            }

            Debug.Log("SimpleTcpClient sending " + message + " to " + ipAddress);

            this.BeginSendMessageToServer(ipAddress, message); 
        }

        public void ConnectTo(string ipAddress, int port, Action OnConnected)
        {
            if (!this.initialized)
            {
                throw new InvalidOperationException("Not initialized");
            }

            if (!ipAddress.IsValidIPV4())
            {
                throw new ArgumentException("Invalid Ipv4 address");
            }

            if (port < 0)
            {
                throw new ArgumentOutOfRangeException("port", "Invalid port");
            }

            /*if (ipAddress == "127.0.0.1" || ipAddress == NetworkUtils.GetLocalIP())
        {
            throw new Exception("Cannot connect to self");
        }
          */  
            if (this.connectedToServersIPsSockets.ContainsKey(ipAddress))
            {
                throw new Exception("Already connected to " + ipAddress);
            }

            Debug.Log("SimpleTcpClient connecting to " + ipAddress + ':' + port.ToString());

            this.BeginConnectToServer(ipAddress, port, OnConnected);
        }

        public void DisconnectFrom(string ipAddress)
        {
            if (!this.connectedToServersIPsSockets.ContainsKey(ipAddress))
            {
                throw new Exception("Not connected to " + ipAddress);
            }   

            var socket = this.connectedToServersIPsSockets[ipAddress];
            var state = new DisconnectState(ipAddress, socket);

            Debug.Log("SimpleTcpClient disconnecting to " + ipAddress);

            socket.BeginDisconnect(false, this.EndDisconnect, state);
        }

        void EndDisconnect(IAsyncResult result)
        {
            var state = (DisconnectState)result.AsyncState;
            var socket = state.Socket;

            if (!socket.Connected)
            {
                return;
            }

            try
            {
                this.connectedToServersIPsSockets.Remove(state.IPAddress);
                socket.Close();
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }

            Debug.Log("SimpleTcpClient disconnected from " + state.IPAddress);
        }

        public void Initialize()
        {
            if (this.initialized)
            {
                throw new InvalidOperationException("Already intialzied");
            }

            var threadUtils = ThreadUtils.Instance;//initialize

            this.CoroutineUtils.RepeatEverySeconds(1f, this.UpdateConnectedSockets);
            this.initialized = true;

            Debug.Log("SimpleTcpClient initialized");
        }

        public void Dispose()
        {
            this.StopAllCoroutines();

            var connectedSockets = this.connectedToServersIPsSockets.ToList();
            connectedSockets.ForEach(ipSocket =>
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

            Debug.Log("SimpleTcpClient disposed");
        }

        public bool IsConnectedTo(string ipAddress)
        {
            return this.connectedToServersIPsSockets.ContainsKey(ipAddress);
        }
    }

}

