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
    const int AcceptNewConnectionDelayInMiliseconds = 200;
    const float UpdateSocketsDelayInSeconds = 0.1f;
    protected const int ReceiveMessageTimeoutInMiliseconds = 4000;
    protected const int SendMessageTimeoutInMiliseconds = 4000;

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

    void OnDisable()
    {
        Dispose();
    }

    void RemoveDisconnectedSockets()
    {
        var disconnectedSockets = connectedIPClientsSocket.Where(s => !s.Value.Connected).ToList();

        disconnectedSockets.ForEach(ipSocket =>
            {
                try
                {
                    Disconnect(ipSocket.Key);
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.Message);   
                }
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
        var state = new ReceiveMessageState(socket);

        if (!socketsMessageState.ContainsKey(socket))
        {
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
        var bytesReceivedCount = socket.EndReceive(result, out socketState);

        if (bytesReceivedCount == 0 || socketState != SocketError.Success)
        {
            try
            {
                Disconnect(state.IPAddress);
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
            var filteredMessage = FilterReceivedMessage(message);
            var decryptedMessage = CipherUtility.Decrypt<RijndaelManaged>(filteredMessage, SecuritySettings.NETWORK_ENCRYPTION_PASSWORD, SecuritySettings.SALT);
            var args = new MessageEventArgs(state.IPAddress, decryptedMessage);

            Debug.Log("Received " + decryptedMessage + " from " + state.IPAddress);

            if (OnReceivedMessage != null)
            {
                ThreadUtils.Instance.RunOnMainThread(() => OnReceivedMessage(this, args));
            }

            socketsMessageState[socket] = new ReceiveMessageState(socket);
        }

        Thread.Sleep(30);
        socket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, new AsyncCallback(EndReceiveMessage), state);
    }

    public virtual void Initialize(int port)
    {
        if (Initialized)
        {
            throw new InvalidOperationException("Already initialized");
        }

        var threadUtils = ThreadUtils.Instance;//initialize

        this.port = port;
        acceptConnections.ExclusiveAddressUse = false;
        acceptConnections.SendTimeout = SendMessageTimeoutInMiliseconds;
        acceptConnections.ReceiveTimeout = ReceiveMessageTimeoutInMiliseconds;
        acceptConnections.Bind(new IPEndPoint(IPAddress.Any, Port));
        acceptConnections.Listen(40);

        BeginAcceptConnections();

        CoroutineUtils.RepeatEverySeconds(0.5f, RemoveDisconnectedSockets);

        initialized = true;
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
        var state = new DisconnectState(ipAddress, socket);

        socket.BeginDisconnect(false, new AsyncCallback(EndDisconnect), state);
    }

    void EndDisconnect(IAsyncResult result)
    {
        var state = (DisconnectState)result.AsyncState;
        var socket = state.Socket;

        try
        {
            connectedIPClientsSocket.Remove(state.IPAddress);
            socket.EndDisconnect(result);
            socket.Close();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public virtual void Dispose()
    {
        try
        {
            acceptConnections.Close();    
        }
        catch (Exception ex)
        {
            
        }
       
        var connectedIPClientsSockets = connectedIPClientsSocket.ToList();

        connectedIPClientsSockets.ForEach(ipSocket =>
            {
                var ip = ipSocket.Key;
                Disconnect(ip);
            });        
        connectedIPClientsSocket.Clear();

        initialized = false;
    }

    public bool IsClientConnected(string ipAddress)
    {
        return connectedIPClientsSocket.ContainsKey(ipAddress);
    }
    //*/
}
