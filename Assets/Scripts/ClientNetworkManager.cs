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

    public NotificationsController NotificationsController;
    //how many times to try to connect to server before disconnecting and start searching for another (only if LANbroadcastService is present)
    public byte RetriesBeforeSearchingForAnotherServer = 3;

    public EventHandler OnConnectedEvent = delegate
    {
    };

    public EventHandler<DataSentEventArgs> OnReceivedDataEvent = delegate
    {
    };

    public EventHandler OnDisconnectedEvent = delegate
    {
    };


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

    void Start()
    {
        ConfigureClient();
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
    }

    IEnumerator UpdateCoroutine()
    {
        while (true)
        {
            if (isRunning)
            {
                UpdateClient();
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    string GetUsername()
    {
        var username = "";

        if (PlayerPrefs.HasKey("Username"))
        {
            username = PlayerPrefs.GetString("Username");
        }

        return username;
    }

    void UpdateClient()
    {
        NetworkData receiveNetworkData = null;

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

            return;
        }

        switch (receiveNetworkData.NetworkEventType)
        {
            case NetworkEventType.ConnectEvent:
                OnConnectedToServer(receiveNetworkData);
                break;

            case NetworkEventType.DataEvent:
                OnDataReceivedFromServer(receiveNetworkData);
                break;

            case NetworkEventType.DisconnectEvent:                            
                DisconnectedFromServer(receiveNetworkData);
                break;
        }      
    }

    void OnConnectedToServer(NetworkData networkData)
    {
        var username = GetUsername();
        SendMessage("UsernameSet=" + username);

        if (OnConnectedEvent != null)
        {
            OnConnectedEvent(this, EventArgs.Empty);    
        }
    }

    void OnDataReceivedFromServer(NetworkData networkData)
    {
        var message = networkData.Message;
        var username = GetUsername();

        if (OnReceivedDataEvent != null)
        {
            OnReceivedDataEvent(this, new DataSentEventArgs(connectionId, username, message));    
        }
    }

    void DisconnectedFromServer(NetworkData networkData)
    {
        NetworkTransport.Shutdown();
        isRunning = false;

        if (OnDisconnectedEvent != null)
        {
            OnDisconnectedEvent(this, EventArgs.Empty);    
        }
    }

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