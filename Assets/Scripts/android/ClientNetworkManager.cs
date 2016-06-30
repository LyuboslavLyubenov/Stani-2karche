using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

/// <summary>
/// Client network manager.
/// </summary>
public class ClientNetworkManager : MonoBehaviour
{
    const int Port = 7788;

    public GameObject DialogUI;
    public LANBroadcastService broadcastService;
    //how many times to try to connect to server before disconnecting and start searching for another (only if LANbroadcastService is present)
    public byte RetriesBeforeSearchingForAnotherServer = 2;

    DialogUIController dialogUIController = null;

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

    public EventHandler OnConnectedEvent = delegate
    {
        
    };

    public EventHandler<DataSentEventArgs> OnReceivedDataEvent = delegate
    {
        
    };
 
    public EventHandler OnDisconnectedEvent = delegate
    {
        
    };

    void Start()
    {
        if (DialogUI != null)
        {
            dialogUIController = DialogUI.GetComponent<DialogUIController>();
        }

        ConfigureClient();

        if (broadcastService != null)
        {
            broadcastService.OnFound += OnServerFoundCoroutine;
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

    void OnServerFoundCoroutine(object sender, BroadcastIpEventArgs args)
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
            broadcastService.RestartService();
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
                    //if there is a error
                    if (dialogUIController != null)
                    {
                        //get its type
                        var message = (NetworkError)e.ErrorN;

                        //show it on screen
                        DialogUI.SetActive(true);
                        dialogUIController.SetErrorMessage(message);

                        //if cannot connect to server
                        if (message == NetworkError.Timeout)
                        {
                            //disconnect
                            Disconnect();
                        }
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
                            SendData("UsernameSet=" + username);
                            OnConnectedEvent(this, EventArgs.Empty);
                            break;

                        case NetworkEventType.BroadcastEvent:
                            break;

                        case NetworkEventType.DataEvent:
                            var message = receiveNetworkData.Message;
                            OnReceivedDataEvent(this, new DataSentEventArgs(connectionId, username, message));
                            break;

                        case NetworkEventType.DisconnectEvent:                            
                            NetworkTransport.Shutdown();
                            isRunning = false;
                            broadcastService.RestartService();
                            OnDisconnectedEvent(this, EventArgs.Empty);
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
            Disconnect();
            DialogUI.SetActive(true);
            dialogUIController.SetErrorMessage(networkError);
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
            DialogUI.SetActive(true);
            dialogUIController.SetErrorMessage(networkError);
        }

        broadcastService.RestartService();

        OnDisconnectedEvent(this, EventArgs.Empty);
    }

    public void SendData(string data)
    {
        NetworkTransportUtils.SendMessage(genericHostId, connectionId, communicationChannel, data);
    }
}