using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System;
using System.Net.NetworkInformation;
using System.Threading;
using CielaSpike;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Collections.Generic;

public class LANBroadcastService : MonoBehaviour
{
    const int Port = 7777;
    const float TimeDelaySendBroadcastInSeconds = 0.5f;

    const string IAmServer = "Stani2karcheServer";
    const string IAmClient = "Stani2karcheClient";

    public BroadcastType broadcastType = BroadcastType.Client;

    public EventHandler<BroadcastIpEventArgs> OnFound = delegate
    {
    };

    UdpClient udpClient = null;
    IPEndPoint listenEndPoint = new IPEndPoint(IPAddress.Any, Port);

    bool isRunning = false;

    void Start()
    {
        Initialize();
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
        udpClient.EnableBroadcast = true;
        udpClient.Client.EnableBroadcast = true;
        udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        udpClient.Client.Bind(endPoint);
    }

    void OnDisable()
    {
        Dispose();
    }

    void Dispose()
    {
        StopAllCoroutines();
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
            var message = ConvertAndFilterBuffer(buffer);
       
            yield return Ninja.JumpToUnity;

            if (!string.IsNullOrEmpty(message) && message.Length > 0 && message.Equals(IAmServer))
            {
                //TODO: TELL THE SERVER THAT YOU FOUND IT
                OnFound(this, new BroadcastIpEventArgs(ip.Address.ToString()));
                isRunning = false;       
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

    string ConvertAndFilterBuffer(byte[] buffer)
    {
        if (buffer == null)
        {
            return "";
        }

        var message = System.Text.Encoding.UTF8.GetString(buffer).ToCharArray();
        var result = new System.Text.StringBuilder();

        foreach (var c in message)
        {
            if (c != '\0')
            {
                result.Append(c);
            }
        }

        return result.ToString();
    }

    public void RestartService()
    {
        Dispose();
        Initialize();
    }
}

