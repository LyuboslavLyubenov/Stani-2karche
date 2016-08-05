using UnityEngine;
using System.Net.Sockets;

public class TESTBasicExamGameInfoSender : ExtendedMonoBehaviour
{
    const int Port = 4444;

    public CreatedGameInfoSenderService Sender;
    public ServerNetworkManager ServerNetworkManager;
    public SimpleTcpServer TcpServer;

    void Start()
    {
        ServerNetworkManager.StartServer();
        TcpServer.Initialize(Port);
    }
}
