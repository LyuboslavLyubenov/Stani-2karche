using UnityEngine;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Collections;
using System;
using CielaSpike;
using System.Threading;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

public class P2PSocket : SimpleTcpServer
{
    Dictionary<string, Socket> connectedToServersIPsSockets = new Dictionary<string, Socket>();

    readonly object MyLock = new object();

    IEnumerator UpdateConnectedSockets()
    {
        while (true)
        {
            Thread.Sleep(100);

            lock (MyLock)
            {
                connectedToServersIPsSockets = connectedToServersIPsSockets.Where(ipSocket => ipSocket.Value.Connected)
                    .ToDictionary(ipSocket => ipSocket.Key, ipSocket => ipSocket.Value);
            }
        }
    }

    IEnumerator ConnectToServerCoroutineAsync(string ipAddress, int port, Action<IpEventArgs> OnConnected = null, Action<string> OnError = null)
    {
        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        socket.SendTimeout = ReceiveMessageTimeoutInMiliseconds;
        socket.ReceiveTimeout = SendMessageTimeoutInMiliseconds;

        string exceptionMessage = string.Empty;

        try
        {
            socket.Connect(ipAddress, port);    
        }
        catch (SocketException ex)
        {
            exceptionMessage = ex.Message;
            Debug.Log(ex.Message);
        }

        if (exceptionMessage != string.Empty)
        {
            yield return Ninja.JumpToUnity;

            if (OnError != null)
            {
                OnError(exceptionMessage);
            }
        }

        var ipEventArgs = new IpEventArgs(ipAddress);

        yield return Ninja.JumpToUnity;

        connectedToServersIPsSockets.Add(ipAddress, socket);

        if (OnConnected != null)
        {
            OnConnected(ipEventArgs);    
        }
    }

    IEnumerator SendMessageToServerCoroutineAsync(string ipAddress, string message, Action OnSent = null, Action<string> OnError = null)
    {
        var socket = connectedToServersIPsSockets[ipAddress];
        var encryptedMessage = CipherUtility.Encrypt<RijndaelManaged>(message, ENCRYPTION_PASSWORD, ENCRYPTION_SALT);
        var buffer = Encoding.UTF8.GetBytes(encryptedMessage);

        try
        {
            socket.Send(buffer);    
        }
        catch (SocketException ex)
        {
            if (OnError != null)
            {
                OnError(ex.Message);
            }

            yield break;
        }

        yield return Ninja.JumpToUnity;

        if (OnSent != null)
        {
            OnSent();
        }
    }

    public override void Send(string ipAddress, string message, Action OnSent = null, Action<string> OnError = null)
    {
        if (connectedToServersIPsSockets.ContainsKey(ipAddress))
        {
            this.StartCoroutineAsync(SendMessageToServerCoroutineAsync(ipAddress, message, OnSent, OnError));    
        }
        else
        {
            base.Send(ipAddress, message, OnSent, OnError);
        }
    }

    public override void SendToAll(string message, Action<string> OnError)
    {
        if (connectedToServersIPsSockets.Count > 0)
        {
            connectedToServersIPsSockets.Keys.ToList().ForEach(ip => Send(ip, message, null, OnError));
        }
        else
        {
            base.SendToAll(message, OnError);
        }
    }

    public void ConnectTo(string ipAddress, int port, Action<IpEventArgs> OnConnected = null, Action<string> OnError = null)
    {
        if (!base.Initialized)
        {
            OnError("Not initialized");
        }

        if (!ipAddress.IsValidIPV4())
        {
            OnError("Invalid Ipv4 address");
            return;
        }

        if (port < 0)
        {
            OnError("Invalid port");
            return;
        }

        if (connectedToServersIPsSockets.ContainsKey(ipAddress))
        {
            OnError("Already connected to client");
            return;
        }

        if (ipAddress == "127.0.0.1" || ipAddress == NetworkUtils.GetLocalIP())
        {
            OnError("Cannot connect to self");
            return;
        }

        this.StartCoroutineAsync(ConnectToServerCoroutineAsync(ipAddress, port, OnConnected, OnError));
    }

    public override void Initialize(int port)
    {
        base.Initialize(port);
        this.StartCoroutineAsync(UpdateConnectedSockets());
    }
}
