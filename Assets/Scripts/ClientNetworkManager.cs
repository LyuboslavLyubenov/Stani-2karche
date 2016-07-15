using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

/// <summary>
/// Client network manager.
/// </summary>
public class ClientNetworkManager : MonoBehaviour, INetworkManager
{
    const int Port = 7788;

    public LANBroadcastService BroadcastService;
    public NotificationsController NotificationsController;
    //how many times to try to connect to server before disconnecting and start searching for another (only if LANbroadcastService is present)
    public byte RetriesBeforeSearchingForAnotherServer = 3;

    int connectionId = 0;
    int genericHostId = 0;

    ConnectionConfig connectionConfig = null;
    byte communicationChannel = 0;

    bool isRunning = false;

    public bool IsRunning
    {
        get
        {
            return isRunning;
        }
    }

    public EventHandler OnConnectedEvent
    {
        get;
        set;
    }

    public EventHandler<DataSentEventArgs> OnReceivedDataEvent
    {
        get;
        set;
    }

    public EventHandler OnDisconnectedEvent
    {
        get;
        set;
    }

    void Start()
    {
        ConfigureClient();

        if (BroadcastService != null)
        {
            BroadcastService.OnFound += OnServerFoundCoroutine;
        }

        StartCoroutine(UpdateCoroutine());
    }

    /// <summary>
    /// Configures the client connection settings.
    /// </summary>
    void ConfigureClient()
    {
        connectionConfig = new ConnectionConfig();
        connectionConfig.MaxConnectionAttempt = RetriesBeforeSearchingForAnotherServer; 
        communicationChannel = connectionConfig.AddChannel(QosType.ReliableSequenced); //make sure messages are delivered and send in correct order
    }

    void OnServerFoundCoroutine(object sender, IpEventArgs args)
    {
        //if server found try to connect
        StartCoroutine(OnServerFoundCoroutine(args.IPAddress));   
    }

    IEnumerator OnServerFoundCoroutine(string address)
    {
        bool successfullyConnected = false;

        for (int i = 0; i < RetriesBeforeSearchingForAnotherServer; i++)
        {
            //try to connect
            ConnectToHost(address);

            yield return new WaitForSeconds(0.5f);

            //if connected dont try again
            if (isRunning)
            {
                successfullyConnected = true;
                break;
            }
        }

        if (!successfullyConnected)
        {
            //if we were unable to connect to the host, restart broadcast service and start searching for new server
            BroadcastService.RestartService();
        }
    }

    //used for receiving messages from server
    IEnumerator UpdateCoroutine()
    {
        while (true)
        {
            if (isRunning)
            {
                NetworkData receiveNetworkData = null;
                var hasError = false;

                try
                {
                    receiveNetworkData = NetworkTransportUtils.ReceiveMessage();
                }
                catch (NetworkException e)
                {
                    var error = (NetworkError)e.ErrorN;
                    var errorMessage = NetworkErrorUtils.GetMessage(error);

                    NotificationsController.AddNotification(Color.red, errorMessage);

                    //if cannot connect to server
                    if (error == NetworkError.Timeout)
                    {
                        //disconnect
                        Disconnect();
                    }
           
                    hasError = true;
                }

                if (!hasError)
                {
                    var username = "";

                    if (PlayerPrefs.HasKey("Username"))
                    {
                        username = PlayerPrefs.GetString("Username");
                    }

                    switch (receiveNetworkData.NetworkEventType)
                    {
                        case NetworkEventType.Nothing:
                            //nothing happend
                            break;

                        case NetworkEventType.ConnectEvent:
                            //if connected to server
                            //send our username
                            SendMessage("UsernameSet=" + username);

                            if (OnConnectedEvent != null)
                            {
                                OnConnectedEvent(this, EventArgs.Empty);    
                            }

                            break;

                        case NetworkEventType.BroadcastEvent:
                            break;

                        case NetworkEventType.DataEvent:
                            var message = receiveNetworkData.Message;

                            if (OnReceivedDataEvent != null)
                            {
                                OnReceivedDataEvent(this, new DataSentEventArgs(connectionId, username, message));    
                            }

                            break;

                        case NetworkEventType.DisconnectEvent:                            
                            NetworkTransport.Shutdown();
                            isRunning = false;
                            BroadcastService.RestartService();

                            if (OnDisconnectedEvent != null)
                            {
                                OnDisconnectedEvent(this, EventArgs.Empty);    
                            }

                            break;
                    }      
                }
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    /// <summary>
    /// Connects to host.
    /// </summary>
    /// <param name="ip">Ip</param>
    public void ConnectToHost(string ip)
    {
        if (string.IsNullOrEmpty(ip) && ip.Length < 4)
        {
            throw new ArgumentOutOfRangeException("ip", "Invalid ip address length");
        }
            
        var ipDigits = ip.Split('.');
       
        if (ipDigits.Length != 4)
        {
            throw new ArgumentException("Invalid ip address");
        }

        //if currently connected, disconnect
        if (isRunning)
        {
            Disconnect();
        }

        NetworkTransport.Init();
        HostTopology topology = new HostTopology(connectionConfig, 2);
        genericHostId = NetworkTransport.AddHost(topology, 0);

        byte error;
        connectionId = NetworkTransport.Connect(genericHostId, ip, Port, 0, out error);

        var networkError = (NetworkConnectionError)error;

        if (networkError != NetworkConnectionError.NoError)
        {
            var errorMessage = NetworkErrorUtils.GetMessage(networkError);
            NotificationsController.AddNotification(Color.red, errorMessage);
            Disconnect();
        }
        else
        {
            isRunning = true;    
        }

    }

    public void Disconnect()
    {
        byte error;
        isRunning = false;
        NetworkTransport.Disconnect(genericHostId, connectionId, out error);
        NetworkTransport.RemoveHost(genericHostId);
        NetworkTransport.Shutdown();

        var networkError = (NetworkError)error;

        if (networkError != NetworkError.Ok)
        {
            var errorMessage = NetworkErrorUtils.GetMessage(networkError);
            NotificationsController.AddNotification(Color.red, errorMessage);
        }

        BroadcastService.RestartService();

        if (OnDisconnectedEvent != null)
        {
            OnDisconnectedEvent(this, EventArgs.Empty);    
        }
    }

    public void SendMessage(string data)
    {
        NetworkTransportUtils.SendMessage(genericHostId, connectionId, communicationChannel, data);
    }
}