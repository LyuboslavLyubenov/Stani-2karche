using UnityEngine;

public class TESTRECEIVE : MonoBehaviour
{
    public const int Port = 7777;
    public SimpleTcpServer tcpServer;
    public NotificationsServiceController notifications;

    void Start()
    {
        tcpServer.Initialize(Port);
        tcpServer.OnReceivedMessage += OnReceivedMessage;
    }

    void OnReceivedMessage(object sender, MessageEventArgs args)
    {
        notifications.AddNotification(Color.green, args.Message);
    }

    void OnDisable()
    {
        tcpServer.Dispose();
    }
}
