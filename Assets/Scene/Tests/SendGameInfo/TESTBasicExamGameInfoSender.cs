using UnityEngine;
using System.Collections;
using System.Net.NetworkInformation;
using System.Net.Sockets;

public class TESTBasicExamGameInfoSender : ExtendedMonoBehaviour
{
    public CreatedGameInfoSenderService Sender;

    void Start()
    {
        CoroutineUtils.WaitForSeconds(1, TESTSend);
    }

    void TESTSend()
    {
        var serverInfo = new ServerInfo_Serializable()
        {
            IPAddress = "127.0.0.1",
            ConnectedClients = 0,
            MaxConnectionsAllowed = 10
        };
        
        var gameInfo = new CreatedGameInfo_Serializable()
        {
            ServerInfo = serverInfo,
            GameType = GameType.BasicExam,
            HostUsername = "Dead4y"
        };
        
        Sender.SendGameInfo("192.168.0.101", gameInfo, delegate
            {
            });
    }

    string GetLocalIPv4(NetworkInterfaceType _type)
    {
        string output = "";
        foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
            {
                foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        output = ip.Address.ToString();
                    }
                }
            }
        }
        return output;
    }
}
