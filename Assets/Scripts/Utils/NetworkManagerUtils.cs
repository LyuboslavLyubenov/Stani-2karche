using UnityEngine.Networking;
using UnityEngine;
using System.Collections;
using System;

public class NetworkManagerUtils : MonoBehaviour
{
    static NetworkManagerUtils instance;

    public static NetworkManagerUtils Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = new GameObject();
                obj.name = "NetworkManagerUtils";
                instance = obj.AddComponent<NetworkManagerUtils>();
            }

            return instance;
        }
    }

    NetworkManagerUtils()
    {
        
    }

    public void IsServerUp(string ip, int port, Action<bool> isUp)
    {
        StartCoroutine(IsServerUpCoroutine(ip, port, isUp));
    }

    IEnumerator IsServerUpCoroutine(string ip, int port, Action<bool> isRunning)
    {
        const int MaxConnectionAttempts = 5;

        var connectionConfig = new ConnectionConfig();
        connectionConfig.MaxConnectionAttempt = MaxConnectionAttempts; 

        var communicationChannel = connectionConfig.AddChannel(QosType.ReliableSequenced);
        var topology = new HostTopology(connectionConfig, 2);
        var genericHostId = NetworkTransport.AddHost(topology, 0);

        byte error;
        var connectionId = NetworkTransport.Connect(genericHostId, ip, port, 0, out error);

        var networkError = (NetworkConnectionError)error;
        bool isUp = false;

        yield return new WaitForSeconds(1f);

        int recvConnectionId;
        int recvChannelId;
        byte[] buffer = new byte[512];
        int recSize;
        byte recError;
        var eventType = NetworkTransport.ReceiveFromHost(genericHostId, out recvConnectionId, out recvChannelId, buffer, buffer.Length, out recSize, out recError);

        isUp = eventType == NetworkEventType.ConnectEvent;

        yield return new WaitForEndOfFrame();

        byte disconnectError;
        NetworkTransport.Disconnect(genericHostId, connectionId, out disconnectError);
        NetworkTransport.RemoveHost(genericHostId);

        yield return new WaitForEndOfFrame();

        isRunning(isUp);
    }

}
