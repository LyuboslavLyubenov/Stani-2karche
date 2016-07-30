using UnityEngine;
using System.Collections;
using System.Text;
using System;

public class TESTReceiveGameInfo : MonoBehaviour
{
    public CreatedGameInfoReceiverService Receiver;

    void Start()
    {
        Receiver.ListenAt("192.168.0.100", OnReceived);
    }

    void OnReceived(GameInfoReceivedData data)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine(data.GameInfo.HostUsername);
        sb.AppendLine(data.GameInfo.ServerInfo.IPAddress);

        Debug.Log(sb.ToString());
    }
	
}
