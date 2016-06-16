using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

public class ClientNetworkManager : MonoBehaviour
{
    const string AlreadyConnectedToServer = "Вече сте свързан към сървъра";
    const string ConnectionFailed = "Проблем при свързване със сървъра";

    const int Port = 7788;

    public GameObject DialogUI;

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
        #if DEVELOPMENT_BUILD
        Application.runInBackground = true;
        #endif

        if (DialogUI != null)
        {
            dialogUIController = DialogUI.GetComponent<DialogUIController>();
        }

        connectionConfig = new ConnectionConfig();
        connectionConfig.MaxConnectionAttempt = 3;
        communicationChannel = connectionConfig.AddChannel(QosType.ReliableSequenced);

        StartCoroutine(UpdateCoroutine());
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
                    }

                    hasError = true;
                }

                if (!hasError)
                {
                    switch (recieveNetworkData.NetworkEventType)
                    {
                        case NetworkEventType.Nothing:

                            break;

                        case NetworkEventType.ConnectEvent:
                            var username = PlayerPrefs.GetString("Username");
                            SendData("UsernameSet=" + username);
                            OnConnectedEvent(this, EventArgs.Empty);
                            break;

                        case NetworkEventType.DataEvent:
                            var message = recieveNetworkData.Message;
                            OnRecievedDataEvent(this, new DataSentEventArgs(connectionId, null, message));
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
        byte error;
        genericHostId = NetworkTransport.AddHost(topology, 0);
        connectionId = NetworkTransport.Connect(genericHostId, ip, Port, 0, out error);
        isRunning = true;
    }


    public void Disconnect()
    {
        byte error;
        isRunning = false;
        NetworkTransport.Disconnect(genericHostId, connectionId, out error);
        NetworkTransport.Shutdown();
    }

    public void SendData(string data)
    {
        NetworkTransportUtils.SendMessage(genericHostId, connectionId, communicationChannel, data);
    }
}