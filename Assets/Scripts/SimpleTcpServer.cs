//
//  SimpleTcpServer.cs
//
//  Author:
//       Любослав Любенов <dead4y@mail.bg>
//
//  Copyright (c) 2016 Dead4y
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

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
    const int ReceiveMessageTimeoutInMiliseconds = 1000;

    public const string ENCRYPTION_PASSWORD = "82144042ef1113d6abc9b58f469cf710";
    public const string ENCRYPTION_SALT = "21a87b0b0eb48a341889bf1cb818db67";

    public EventHandler<IpEventArgs> OnClientConnected = delegate
    {  
    };

    public EventHandler<MessageEventArgs> OnReceivedMessage = delegate
    {
    };

    int port;

    Socket acceptConnections = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    Dictionary<string, Socket> connectedIPClientsSocket = new Dictionary<string, Socket>();

    public int Port
    {
        get
        {
            return port;
        }
    }

    void UpdateClientsSockets()
    {
        lock (connectedIPClientsSocket)
        {
            connectedIPClientsSocket = connectedIPClientsSocket.Where(s => s.Value.Connected).ToDictionary(k => k.Key, v => v.Value);
        }
    }

    IEnumerator UpdateGetDataFromSocketsCoroutine()
    {
        Dictionary<string, Socket> connectedClients;

        while (true)
        {
            Thread.Sleep(100);

            lock (connectedIPClientsSocket)
            {
                connectedClients = new Dictionary<string, Socket>(connectedIPClientsSocket);    
            }

            for (int i = 0; i < connectedClients.Keys.Count; i++)
            {
                var ip = connectedClients.Keys.ElementAt(i);
                var socket = connectedClients[ip];

                if (!socket.Connected)
                {
                    lock (connectedIPClientsSocket)
                    {
                        connectedIPClientsSocket.Remove(ip);
                    }

                    continue;
                }

                var receivedBytes = new List<byte>();

                while (socket.Available > 0)
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
                }

                if (receivedBytes.Count <= 0)
                {
                    continue;
                }

                var message = Encoding.UTF8.GetString(receivedBytes.ToArray());
                var filteredMessage = FilterReceivedMessage(message);

                yield return Ninja.JumpToUnity;

                OnReceivedMessage(this, new MessageEventArgs(ip, filteredMessage));

                yield return Ninja.JumpBack;

                Thread.Sleep(30);
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

        var decryptedMessage = CipherUtility.Decrypt<RijndaelManaged>(filtered.ToString(), ENCRYPTION_PASSWORD, ENCRYPTION_SALT);
        return decryptedMessage;
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
        connectedIPClientsSocket.Add(ip, connectionSocket);

        Thread.Sleep(AcceptNewConnectionDelayInMiliseconds);
        BeginAcceptConnections();
    }

    public void Initialize(int port)
    {
        this.port = port;
        acceptConnections.ExclusiveAddressUse = false;
        acceptConnections.Bind(new IPEndPoint(IPAddress.Any, Port));
        acceptConnections.Listen(40);

        CoroutineUtils.RepeatEverySeconds(0.2f, UpdateClientsSockets);
        BeginAcceptConnections();
        this.StartCoroutineAsync(UpdateGetDataFromSocketsCoroutine());
    }

    //returns true if successfully disconnected client
    public bool DisconnectClient(string ipAddress)
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

        lock (connectedIPClientsSocket)
        {
            connectedIPClientsSocket.Remove(ipAddress);
        }

        return true;
    }
    //*/
}