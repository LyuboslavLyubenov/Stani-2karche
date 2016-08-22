using UnityEngine;

public class TCPCLIENTTEST : ExtendedMonoBehaviour
{
    public SimpleTcpClient tcpClient;

    void InitializeAndSendMessageTest()
    {
        tcpClient.Initialize();
        tcpClient.ConnectTo("127.0.0.1", TESTRECEIVE.Port, () => tcpClient.Send("127.0.0.1", "ivan e mnogo gotin"));
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(100, 100, 200, 170), "Initialize and send"))
        {
            InitializeAndSendMessageTest();
        }
    }
}
