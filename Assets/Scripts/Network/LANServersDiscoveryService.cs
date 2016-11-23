using System;

public class LANServersDiscoveryService : LANBroadcastService
{
    public EventHandler<IpEventArgs> OnFound = delegate
    {
    };

    const int RetrieveMessageDelayInSeconds = 1;

    void Start()
    {
        base.Initialize();
        CoroutineUtils.WaitForFrames(1, ReceiveIsServerOnlineMessage);
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

        CoroutineUtils.WaitForSeconds(RetrieveMessageDelayInSeconds, ReceiveIsServerOnlineMessage);
    }
	
}
