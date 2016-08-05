//
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
    const int AcceptNewConnectionDelayInMiliseconds = 60;
    const int UpdateSocketsDelayInMiliseconds = 100;
    const float UpdateAliveSocketsDelayInSeconds = 0.5f;
    protected const int ReceiveMessageTimeoutInMiliseconds = 500;
    protected const int SendMessageTimeoutInMiliseconds = 500;

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

    Socket acceptConnections = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    Dictionary<string, Socket> connectedIPClientsSocket = new Dictionary<string, Socket>();

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

    void UpdateClientsSockets()
    {
        lock (MyLock)
        {
            connectedIPClientsSocket = connectedIPClientsSocket.Where(s => s.Value.Connected).ToDictionary(k => k.Key, v => v.Value);
        }
    }

    IEnumerator UpdateGetDataFromSocketsCoroutine()
    {
        while (true)
        {
            Thread.Sleep(100);

            lock (MyLock)
            {
                for (int i = 0; i < connectedIPClientsSocket.Keys.Count; i++)
                {
                    Thread.Sleep(30);

                    var ip = connectedIPClientsSocket.Keys.ElementAt(i);
                    var socket = connectedIPClientsSocket[ip];

                    if (!socket.Connected)
                    {
                        connectedIPClientsSocket.Remove(ip);
                        continue;
                    }

                    var receivedBytes = new List<byte>();

                    do
                    {
                        var buffer = new byte[2048];

                        try
                        {
                            socket.Receive(buffer);
                        }
                        catch (Exception ex)
                        {
                            Debug.Log(ex.Message);
                            break;
                        }

                        receivedBytes.AddRange(buffer);

                    } while(socket.Available > 0);

                    if (receivedBytes.Count <= 0)
                    {
                        continue;
                    }

                    var message = Encoding.UTF8.GetString(receivedBytes.ToArray());
                    var filteredMessage = FilterReceivedMessage(message);
                    var encryptedMessage = CipherUtility.Encrypt<RijndaelManaged>(filteredMessage, ENCRYPTION_PASSWORD, ENCRYPTION_SALT);

                    yield return Ninja.JumpToUnity;

                    OnReceivedMessage(this, new MessageEventArgs(ip, encryptedMessage));

                    yield return Ninja.JumpBack;
                }
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
        var connectionSocket = socket.EndAccept(result);
        var ip = (connectionSocket.RemoteEndPoint as IPEndPoint).ToString();

        connectionSocket.ReceiveTimeout = ReceiveMessageTimeoutInMiliseconds;
        connectionSocket.SendTimeout = SendMessageTimeoutInMiliseconds;

        lock (MyLock)
        {
            connectedIPClientsSocket.Add(ip, connectionSocket);    
        }

        Thread.Sleep(AcceptNewConnectionDelayInMiliseconds);
        BeginAcceptConnections();
    }

    void _SendMessage(string ipAddress, string message, Action OnSent = null)
    {
        lock (MyLock)
        {
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
            var buffer = Encoding.UTF8.GetBytes(encryptedMessage);

            socket.Send(buffer);        
        }

        if (OnSent != null)
        {
            OnSent();    
        }
    }

    public void Initialize(int port)
    {
        this.port = port;
        acceptConnections.ExclusiveAddressUse = false;
        acceptConnections.Bind(new IPEndPoint(IPAddress.Any, Port));
        acceptConnections.Listen(40);

        CoroutineUtils.RepeatEverySeconds(UpdateAliveSocketsDelayInSeconds, UpdateClientsSockets);
        BeginAcceptConnections();
        this.StartCoroutineAsync(UpdateGetDataFromSocketsCoroutine());
        initialized = true;
    }

    IEnumerator SendMessageAsyncCoroutine(string ipAddress, string message, Action OnSent = null, Action<string> OnError = null)
    {
        try
        {
            _SendMessage(ipAddress, message, OnSent, OnError);    
        }
        catch (Exception ex)
        {
            if (OnError != null)
            {
                OnError(ex.Message);
            }
        }

        yield return null;
    }

    public virtual void Send(string ipAddress, string message, Action OnSent = null, Action<string> OnError = null)
    {
        if (!initialized)
        {
            if (OnError != null)
            {
                OnError("Not initialized");    
            }

            return;
        }

        this.StartCoroutineAsync(SendMessageAsyncCoroutine(ipAddress, message, OnSent, OnError));
    }

    public virtual void SendToAll(string message, Action<string> OnError)
    {
        if (!initialized)
        {
            if (OnError != null)
            {
                OnError("Not initialized");    
            }

            return;
        }

        lock (MyLock)
        {
            if (connectedIPClientsSocket.Count <= 0)
            {
                if (OnError != null)
                {
                    OnError("Not connected to anybody");
                }

                return;
            }

            connectedIPClientsSocket.Keys.ToList().ForEach(ip => Send(ip, message, null, OnError));
        }
    }

    //returns true if successfully disconnected client
    public virtual bool DisconnectFrom(string ipAddress)
    {
        if (!initialized)
        {
            throw new Exception("Use initialize method first");
        }

        lock (MyLock)
        {
            if (!connectedIPClientsSocket.ContainsKey(ipAddress))
            {
                return false;    
            }

            var socket = connectedIPClientsSocket[ipAddress];

            try
            {
                socket.Disconnect(false);    
            }
            catch
            {
            }

            connectedIPClientsSocket.Remove(ipAddress);
        }

        return true;
    }
    //*/
}