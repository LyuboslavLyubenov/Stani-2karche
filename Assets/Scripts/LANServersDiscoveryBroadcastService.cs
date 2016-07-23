using System;

public class LANServersDiscoveryBroadcastService : LANBroadcastService
{
    public EventHandler<IpEventArgs> OnFound = delegate
    {
    };

    void Start()
    {
        base.Initialize();
        ReceiveIsServerOnlineMessage();
    }

    void ReceiveIsServerOnlineMessage()
    {
        base.ReceiveBroadcastMessageAsync(ReceivedBroadcastMessage);
    }

    void ReceivedBroadcastMessage(string ip, string message)
    {
        if (message.Equals(LANServerOnlineBroadcastService.MessageIAmServer))
        {
            OnFound(this, new IpEventArgs(ip));
        }

        ReceiveIsServerOnlineMessage();
    }
	
}
