using UnityEngine;
using System.Text;

public class TESTReceiveGameInfo : MonoBehaviour
{
    const int Port = 4444;

    public CreatedGameInfoReceiverService Receiver;
    public SimpleTcpServer TcpServer;

    void Start()
    {
        TcpServer.Initialize(Port);
        Receiver.ListenAt("192.168.0.104", OnReceived);
    }

    void OnReceived(GameInfoReceivedDataEventArgs data)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine(data.GameInfo.HostUsername);
        sb.AppendLine(data.GameInfo.ServerInfo.LocalIPAddress);

        Debug.Log(sb.ToString());
    }
	
}
