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

public class CreatedGameInfoReceiverService : ExtendedMonoBehaviour
{
    const int Port = 7773;

    Socket acceptConnections = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    Dictionary<string, Action<GameInfoReceivedData>> pendingIPReceiveDataJson = new Dictionary<string, Action<GameInfoReceivedData>>();
    List<Socket> connectedIPClientsSockets = new List<Socket>();

    protected Dictionary<string, Action<GameInfoReceivedData>> PendingIPReceiveDataJson
    {
        get
        {
            lock (pendingIPReceiveDataJson)
            {
                return new Dictionary<string, Action<GameInfoReceivedData>>(pendingIPReceiveDataJson);
            }
        }
    }

    protected List<Socket> ConnectedIPClientsSockets
    {
        get
        {
            lock (connectedIPClientsSockets)
            {
                return new List<Socket>(connectedIPClientsSockets);
            }
        }
    }

    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        acceptConnections.ExclusiveAddressUse = false;
        acceptConnections.Bind(new IPEndPoint(IPAddress.Any, Port));
        acceptConnections.Listen(40);

        CoroutineUtils.RepeatEverySeconds(0.2f, UpdateClientsSockets);
        BeginAcceptConnections();
        this.StartCoroutineAsync(UpdateGetDataFromSocketsCoroutine());
    }

    void UpdateClientsSockets()
    {
        lock (connectedIPClientsSockets)
        {
            connectedIPClientsSockets = connectedIPClientsSockets.Where(s => s.Connected).ToList();
        }
    }

    IEnumerator UpdateGetDataFromSocketsCoroutine()
    {
        while (true)
        {
            Thread.Sleep(100);

            List<Socket> connectedClients = ConnectedIPClientsSockets;
            Dictionary<string, Action<GameInfoReceivedData>> pendingIPs = PendingIPReceiveDataJson;

            for (int i = 0; i < connectedClients.Count; i++)
            {
                var socket = connectedClients[i];

                if (!socket.Connected)
                {
                    lock (connectedIPClientsSockets)
                    {
                        connectedIPClientsSockets.Remove(socket);
                    }

                    continue;
                }

                var ip = (socket.RemoteEndPoint as IPEndPoint).Address.ToString();

                if (!pendingIPs.ContainsKey(ip))
                {
                    DisconnectClientSocket(socket);
                    continue;
                }

                var buffer = new byte[2048];

                try
                {
                    socket.Receive(buffer);    
                }
                catch (SocketException ex)
                {
                    Debug.Log(ex.Message);
                    continue;
                }

                var message = Encoding.UTF8.GetString(buffer);
                var filteredMessage = FilterReceivedMessage(message);
                const string GameInfoTag = "[CreatedGameInfo]";

                if (!filteredMessage.Contains(GameInfoTag))
                {
                    continue;  
                }

                filteredMessage = filteredMessage.Replace(GameInfoTag, ""); 

                GameInfoReceivedData data = null;

                try
                {
                    data = new GameInfoReceivedData(filteredMessage);    
                }
                catch
                {
                    continue;
                }

                yield return Ninja.JumpToUnity;

                if (pendingIPs[ip] != null)
                {
                    pendingIPs[ip].Invoke(data); 
                }

                yield return Ninja.JumpBack;

                Thread.Sleep(30);

                DisconnectClientSocket(socket);

                lock (pendingIPReceiveDataJson)
                {
                    pendingIPReceiveDataJson.Remove(ip);
                }
                 
            }
        }
    }

    void DisconnectClientSocket(Socket socket)
    {
        try
        {
            socket.Disconnect(false);    
        }
        catch
        {
            
        }

        lock (connectedIPClientsSockets)
        {
            connectedIPClientsSockets.Remove(socket);
        }
    }

    void BeginAcceptConnections()
    {
        acceptConnections.BeginAccept(new AsyncCallback(EndAcceptConnections), acceptConnections);
    }

    void EndAcceptConnections(IAsyncResult result)
    {
        var socket = (Socket)result.AsyncState;
        var connectionSocket = socket.EndAccept(result);
        var clientIP = (connectionSocket.RemoteEndPoint as IPEndPoint).Address.ToString();
        var pendingIP = PendingIPReceiveDataJson;
        var connectedIP = ConnectedIPClientsSockets;

        Debug.Log("Trying to connect " + clientIP);

        if (pendingIP.ContainsKey(clientIP))
        {
            Debug.Log("Connected " + clientIP + " " + pendingIP.ContainsKey(clientIP));

            lock (connectedIPClientsSockets)
            {
                connectedIPClientsSockets.Add(connectionSocket);
            }
        }
        else if (!connectedIP.Contains(connectionSocket))
        {
            connectionSocket.Disconnect(false);
        }

        Thread.Sleep(100);
        BeginAcceptConnections();
    }

    string FilterReceivedMessage(string message)
    {
        var result = new StringBuilder();

        for (int i = 0; i < message.Length; i++)
        {
            if (message[i] != '\0')
            {
                result.Append(message[i]);
            }
        }

        return result.ToString();
    }

    public void ListenAt(string ip, Action<GameInfoReceivedData> onReceived)
    {
        lock (pendingIPReceiveDataJson)
        {
            if (!pendingIPReceiveDataJson.ContainsKey(ip))
            {
                pendingIPReceiveDataJson.Add(ip, onReceived);    
            }
        }

        Debug.Log("ListeningAt " + ip);
    }

    public void RemoveListener(string ip)
    {
        lock (pendingIPReceiveDataJson)
        {
            if (pendingIPReceiveDataJson.ContainsKey(ip))
            {
                pendingIPReceiveDataJson.Remove(ip);
            }
        }

        lock (connectedIPClientsSockets)
        {
            var socketIndex = connectedIPClientsSockets.FindIndex(
                                  delegate(Socket socket)
                {
                    var socketIpEndPoint = socket.RemoteEndPoint as IPEndPoint;
                    return socketIpEndPoint.Address.ToString() == ip;
                });


            if (socketIndex != -1)
            {
                var socket = connectedIPClientsSockets[socketIndex];
                socket.Disconnect(false);
                connectedIPClientsSockets.Remove(socket);
            }
        }
    }

    public void ClearAllListeners()
    {
        StopAllCoroutines();

        lock (pendingIPReceiveDataJson)
        {
            pendingIPReceiveDataJson.Clear();    
        }

        lock (connectedIPClientsSockets)
        {
            for (int i = 0; i < connectedIPClientsSockets.Count; i++)
            {
                var socket = connectedIPClientsSockets[i];
                socket.Disconnect(false);
            }

            connectedIPClientsSockets.Clear();
        }

        Initialize();
    }

    public bool IsListening(string ip)
    {
        lock (pendingIPReceiveDataJson)
        {
            return pendingIPReceiveDataJson.ContainsKey(ip);
        }
    }

    //*/
}