using UnityEngine;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using CielaSpike;
using System.Net;
using System;
using System.Text;
using System.Linq;
using System.Threading;
using System.Security.Cryptography;

public class SimpleTcpServer : ExtendedMonoBehaviour
{
    const int AcceptNewConnectionDelayInMiliseconds = 100;
    const float UpdateSocketsDelayInSeconds = 0.1f;
    protected const int ReceiveMessageTimeoutInMiliseconds = 0;
    protected const int SendMessageTimeoutInMiliseconds = 0;

    public const string ENCRYPTION_PASSWORD = "82144042ef1113d6abc9b58f469cf710";
    public const string ENCRYPTION_SALT = "21a87b0b0eb48a341889bf1cb818db67";

    public EventHandler<IpEventArgs> OnClientConnected = delegate
    {
    };

    public EventHandler<MessageEventArgs> OnReceivedMessage = delegate
    {
    };

    readonly object MyLock = new object();

    int port;
    bool initialized = false;

    readonly Socket acceptConnections = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    Dictionary<string, Socket> connectedIPClientsSocket = new Dictionary<string, Socket>();
    Dictionary<Socket, ReceiveMessageState> socketsMessageState = new Dictionary<Socket, ReceiveMessageState>();

    public bool Initialized
    {
        get
        {
            return initialized;
        }
    }

    public int Port
    {
        get
        {
            return port;
        }
    }

    void RemoveDisconnectedSockets()
    {
        if (Monitor.TryEnter(MyLock, 1000))
        {
            try
            {
                connectedIPClientsSocket = connectedIPClientsSocket.Where(s => s.Value.Connected).ToDictionary(k => k.Key, v => v.Value);    
            }
            finally
            {
                Monitor.Exit(MyLock);
            }
        }
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
        acceptConnections.BeginAccept(new AsyncCallback(EndAcceptConnections), acceptConnections);
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

            connectedIPClientsSocket.Add(ip, connectionSocket);

            Debug.Log("Accepted " + ip);

            BeginReceiveMessage(ip);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        finally
        {
            Thread.Sleep(AcceptNewConnectionDelayInMiliseconds);
            BeginAcceptConnections();
        }
    }

    void BeginSendMessageToClient(string ipAddress, string message)
    {
        if (!initialized)
        {
            throw new InvalidOperationException("Not initialized");
        }

        if (!connectedIPClientsSocket.ContainsKey(ipAddress))
        {
            throw new ArgumentException("Not connected to " + ipAddress, "ipAddress");
        }

        var socket = connectedIPClientsSocket[ipAddress];

        if (!socket.Connected)
        {
            throw new Exception("Connection problem");
        }

        var encryptedMessage = CipherUtility.Encrypt<RijndaelManaged>(message, ENCRYPTION_PASSWORD, ENCRYPTION_SALT);
        var messageBuffer = Encoding.UTF8.GetBytes(encryptedMessage);
        var prefix = BitConverter.GetBytes(messageBuffer.Length);
        var state = new SendMessageState() { Client = socket, DataToSend = new byte[messageBuffer.Length + 4] };

        Buffer.BlockCopy(prefix, 0, state.DataToSend, 0, prefix.Length);
        Buffer.BlockCopy(messageBuffer, 0, state.DataToSend, prefix.Length, messageBuffer.Length);

        socket.BeginSend(messageBuffer, 0, messageBuffer.Length, SocketFlags.None, new AsyncCallback(EndSendMessageToClient), state);
    }

    void EndSendMessageToClient(IAsyncResult result)
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
                socket.BeginSend(state.DataToSend, state.DataSentLength, sendSize, SocketFlags.None, new AsyncCallback(EndSendMessageToClient), state);
            }
            else
            {
                Debug.Log("Sent " + Encoding.UTF8.GetString(state.DataToSend));
                //TODO: ON SENT MESSAGE EVENT
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);    
        }
    }

    void BeginReceiveMessage(string ipAddress)
    {
        if (!ipAddress.IsValidIPV4())
        {
            throw new ArgumentException("Invalid ipv4 address");
        }

        if (!connectedIPClientsSocket.ContainsKey(ipAddress))
        {
            throw new Exception("Not connected to " + ipAddress);
        }

        var socket = connectedIPClientsSocket[ipAddress];
        ReceiveMessageState state;

        if (socketsMessageState.ContainsKey(socket))
        {
            state = new ReceiveMessageState(socket);
        }
        else
        {
            state = new ReceiveMessageState(socket);
            socketsMessageState.Add(socket, state);
        }

        socket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, new AsyncCallback(EndReceiveMessage), state);
    }

    void EndReceiveMessage(IAsyncResult result)
    {
        var state = (ReceiveMessageState)result.AsyncState;
        var socket = state.Socket;
        var offset = 0;

        SocketError socketState;
        int bytesReceivedCount;

        try
        {
            bytesReceivedCount = socket.EndReceive(result, out socketState);

            if (socketState != SocketError.Success)
            {
                return;
            }

            Debug.Log("Received count " + bytesReceivedCount);

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
                var filteredMessage = FilterReceivedMessage(message);
                var args = new MessageEventArgs(state.IPAddress, filteredMessage);

                Debug.Log(filteredMessage);

                if (OnReceivedMessage != null)
                {
                    OnReceivedMessage(this, args);    
                }

                socketsMessageState[socket] = new ReceiveMessageState(socket);
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
        finally
        {
            socket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, new AsyncCallback(EndReceiveMessage), state);
        }
    }

    public virtual void Initialize(int port)
    {
        if (Initialized)
        {
            throw new InvalidOperationException("Already initialized");
        }

        this.port = port;
        acceptConnections.ExclusiveAddressUse = false;
        acceptConnections.Blocking = false;
        acceptConnections.SendTimeout = SendMessageTimeoutInMiliseconds;
        acceptConnections.ReceiveTimeout = ReceiveMessageTimeoutInMiliseconds;
        acceptConnections.Bind(new IPEndPoint(IPAddress.Any, Port));
        acceptConnections.Listen(40);

        BeginAcceptConnections();

        initialized = true;
    }

    public virtual void Send(string ipAddress, string message)
    {
        BeginSendMessageToClient(ipAddress, message);
    }

    public virtual void SendToAll(string message)
    {
        if (connectedIPClientsSocket.Count <= 0)
        {
            throw new Exception("0 connected clients");
        }    

        connectedIPClientsSocket.Keys.ToList().ForEach(ip => Send(ip, message));
    }

    public virtual void Disconnect(string ipAddress)
    {
        if (!initialized)
        {
            throw new Exception("Use initialize method first");
        }

        if (!connectedIPClientsSocket.ContainsKey(ipAddress))
        {
            throw new ArgumentException("Not connected to " + ipAddress);
        }

        var socket = connectedIPClientsSocket[ipAddress];
        socket.Disconnect(false);
        connectedIPClientsSocket.Remove(ipAddress);
    }

    public virtual void Dispose()
    {
        acceptConnections.Close();
        connectedIPClientsSocket.Values.ToList().ForEach(s => s.Close());
        connectedIPClientsSocket.Clear();

        initialized = false;
    }
    //*/
}
