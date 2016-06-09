using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkTransportUtils : MonoBehaviour
{
    public static NetworkData RecieveMessage()
    {
        int recHostId; 
        int connectionId; 
        int channelId; 
        byte[] recBuffer = new byte[1024]; 
        int bufferSize = 1024;
        int dataSize;
        byte error;
        NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);
        return new NetworkData(connectionId, recBuffer, recData);
    }

    public static void SendMessage(int hostId, int connectionId, int channelId, string message)
    {
        var buffer = System.Text.Encoding.UTF8.GetBytes(message);
        byte error;
        NetworkTransport.Send(hostId, connectionId, channelId, buffer, buffer.Length, out error);
    }
}

