using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

public class ClientNetworkManager : MonoBehaviour
{
    const int Port = 7788;

    public GameObject DialogUI;
    public LANBroadcastService broadcastService;
    public int RetriesBeforeSearchingForAnotherServer = 3;

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

    public EventHandler<DataSentEventArgs> OnRecievedDataEvent = delegate
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
            broadcastService.OnFound += OnServerFound;
        }

        StartCoroutine(UpdateCoroutine());
    }

    void OnServerFound(object sender, BroadcastIpEventArgs args)
    {
        StartCoroutine(OnServerFound(args.IPAddress));   
    }

    IEnumerator OnServerFound(string address)
    {
        bool successfullyConnected = false;

        for (int i = 0; i < RetriesBeforeSearchingForAnotherServer; i++)
        {
            ConnectToHost(address);

            yield return new WaitForSeconds(0.5f);

            if (isRunning)
            {
                successfullyConnected = true;
                break;
            }
        }

        if (!successfullyConnected)
        {
            broadcastService.RestartService();
        }
    }

    void ConfigureClient()
    {
        connectionConfig = new ConnectionConfig();
        connectionConfig.MaxConnectionAttempt = 3;
        communicationChannel = connectionConfig.AddChannel(QosType.ReliableSequenced);
    }

    void HandleErrorMessage(byte error)
    {
        if (dialogUIController != null)
        {
            var errorMessage = (NetworkConnectionError)error;

            if (errorMessage == NetworkConnectionError.NoError)
            {
                isRunning = true;
            }
            else
            {
                DialogUI.SetActive(true);
                dialogUIController.SetErrorMessage(errorMessage);
            }
        }
    }

    IEnumerator UpdateCoroutine()
    {
        while (true)
        {
            if (isRunning)
            {
                NetworkData recieveNetworkData = null;
                var hasError = false;

                try
                {
                    recieveNetworkData = NetworkTransportUtils.RecieveMessage();
                }
                catch (NetworkException e)
                {
                    if (dialogUIController != null)
                    {
                        var message = (NetworkError)e.ErrorN;

                        DialogUI.SetActive(true);
                        dialogUIController.SetErrorMessage(message);

                        if (message == NetworkError.Timeout)
                        {
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
                       
                    switch (recieveNetworkData.NetworkEventType)
                    {
                        case NetworkEventType.Nothing:

                            break;

                        case NetworkEventType.ConnectEvent:
                            SendData("UsernameSet=" + username);
                            OnConnectedEvent(this, EventArgs.Empty);
                            break;

                        case NetworkEventType.BroadcastEvent:
                            break;

                        case NetworkEventType.DataEvent:
                            var message = recieveNetworkData.Message;
                            OnRecievedDataEvent(this, new DataSentEventArgs(connectionId, username, message));
                            break;

                        case NetworkEventType.DisconnectEvent:
                            OnDisconnectedEvent(this, EventArgs.Empty);
                            NetworkTransport.Shutdown();
                            isRunning = false;
                            break;
                    }      
                }
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    public void ConnectToHost(string ip)
    {
        if (ip.Length < 4)
        {
            throw new ArgumentOutOfRangeException("ip", "Invalid ip address length");
        }
            
        var ipDigits = ip.Split('.');
       
        if (ipDigits.Length != 4)
        {
            throw new ArgumentException("Invalid ip address");
        }
            
        if (isRunning)
        {
            Disconnect();
        }

        NetworkTransport.Init();
        HostTopology topology = new HostTopology(connectionConfig, 2);
        genericHostId = NetworkTransport.AddHost(topology, 0);

        byte error;
        connectionId = NetworkTransport.Connect(genericHostId, ip, Port, 0, out error);

        if (error != 0)
        {
            HandleErrorMessage(error);    
        }

        isRunning = true;
    }

    public void Disconnect()
    {
        byte error;
        isRunning = false;
        NetworkTransport.Disconnect(genericHostId, connectionId, out error);
        NetworkTransport.RemoveHost(genericHostId);
        NetworkTransport.Shutdown();

        HandleErrorMessage(error);

        OnDisconnectedEvent(this, EventArgs.Empty);
    }

    public void SendData(string data)
    {
        NetworkTransportUtils.SendMessage(genericHostId, connectionId, communicationChannel, data);
    }
}