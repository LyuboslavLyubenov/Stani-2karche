using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System;
using System.Net.NetworkInformation;
using System.Threading;
using CielaSpike;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// LAN broadcast service.
/// </summary>
public class LANBroadcastService : MonoBehaviour
{
    const int Port = 7777;
    //Only if server
    const float TimeDelaySendBroadcastInSeconds = 0.5f;
    //messages send
    const string IAmServer = "Stani2karcheServer";
    const string IAmClient = "Stani2karcheClient";
    //what type i am
    public BroadcastType broadcastType = BroadcastType.Client;
    public bool StopWhenFirstFound = true;

    public EventHandler<IpEventArgs> OnFound = delegate
    {
    };

    List<string> blacklist = new List<string>();

    UdpClient udpClient = null;
    IPEndPoint listenEndPoint = new IPEndPoint(IPAddress.Any, Port);

    bool isRunning = false;

    void Start()
    {
        Initialize();
    }

    void OnDisable()
    {
        Dispose();
    }

    void Initialize()
    {
        var endPoint = listenEndPoint;
        ConfigUDPCLient(endPoint);

        isRunning = true;

        if (broadcastType == BroadcastType.Client)
        {
            StartUpdatingClient();
        }
        else if (broadcastType == BroadcastType.Server)
        {
            StartUpdatingServer();
        }
    }

    void ConfigUDPCLient(IPEndPoint endPoint)
    {
        udpClient = new UdpClient();
        //enable receiving and sending broadcast
        udpClient.EnableBroadcast = true;
        udpClient.Client.EnableBroadcast = true;
        //lines below basically tell that we gonna receive from/ send to endpoint
        udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        udpClient.Client.Bind(endPoint);
    }

    void Dispose()
    {
        StopAllCoroutines();
        isRunning = false;
        udpClient.Close();
        udpClient = null;
    }

    void StartUpdatingClient()
    {
        this.StartCoroutineAsync(UpdateClient());
    }

    IEnumerator UpdateClient()
    {
        yield return new WaitForEndOfFrame();
        
        while (isRunning)
        {
            IPEndPoint ip = null;
            byte[] buffer = udpClient.Receive(ref ip);
            var message = System.Text.Encoding.UTF8.GetString(buffer);
            var isMessageEmpty = string.IsNullOrEmpty(message) && message.Length <= 0;
            var ipv4 = ip.Address.ToString();
            string[] blacklistCopy = null;

            lock (blacklist)
            {
                blacklist.CopyTo(blacklistCopy);
            }

            yield return Ninja.JumpToUnity;

            if (!isMessageEmpty && message.Equals(IAmServer) && !blacklistCopy.Contains(ipv4))
            {
                //TODO: TELL THE SERVER THAT YOU FOUND IT
                OnFound(this, new IpEventArgs(ip.Address.ToString()));

                if (StopWhenFirstFound)
                {
                    isRunning = false;    
                }
            }

            yield return Ninja.JumpBack;

            Thread.Sleep(10);
        }
    }

    void StartUpdatingServer()
    {
        this.StartCoroutineAsync(UpdateServer());
    }

    IEnumerator UpdateServer()
    {
        yield return new WaitForEndOfFrame();

        while (isRunning)
        {
            var IAmServerMsgBytes = System.Text.Encoding.UTF8.GetBytes(IAmServer);
            udpClient.Send(IAmServerMsgBytes, IAmServerMsgBytes.Length, "255.255.255.255", Port);
            Thread.Sleep((int)(TimeDelaySendBroadcastInSeconds * 1000));
        }
    }

    /// <summary>
    /// Gets IPv4 addresses of all connected to network interfaces(LAN, WIFI, etc...).
    /// </summary>
    /// <returns>The all IPv4 addresses</returns>
    List<string> GetAllIPv4Addresses()
    {
        List<string> output = new List<string>();

        foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (item.OperationalStatus == OperationalStatus.Up)
            {
                foreach (UnicastIPAddressInformation ip in item.GetIPProperties()
                    .UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        output.Add(ip.Address.ToString());
                    }
                }
            }
        }

        return output;
    }

    void ValidateIpAddress(string ipAddress)
    {
        if (string.IsNullOrEmpty(ipAddress))
        {
            throw new ArgumentNullException("ipAddress");
        }

        if (ipAddress.Split('.').Length != 4)
        {
            throw new ArgumentException("Invalid ipv4 address");
        }

    }

    public void RestartService()
    {
        Dispose();
        Initialize();
    }

    public void AddToBlackList(string ipAddress)
    {
        if (broadcastType == BroadcastType.Server)
        {
            throw new InvalidOperationException();
        }

        ValidateIpAddress(ipAddress);

        lock (blacklist)
        {
            blacklist.Add(ipAddress);    
        }
    }

    public void RemoveFromBlacklist(string ipAddress)
    {
        if (broadcastType == BroadcastType.Server)
        {
            throw new InvalidOperationException();
        }

        ValidateIpAddress(ipAddress);

        lock (blacklist)
        {
            blacklist.Remove(ipAddress);
        }
    }

    public void ClearBlackList()
    {
        if (broadcastType == BroadcastType.Server)
        {
            throw new InvalidOperationException();
        }

        lock (blacklist)
        {
            blacklist.Clear();
        }
    }
}

