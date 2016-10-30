public class LANServerOnlineBroadcastService : LANBroadcastService
{
    public const string MessageIAmServer = "Stani2karcheIAmServer";

    const float TimeDelaySendServerIsOnlineInSeconds = 1f;

    public void Start()
    {
        base.Initialize();
        CoroutineUtils.WaitForSeconds(TimeDelaySendServerIsOnlineInSeconds, SendServerOnline);
    }

    void SendServerOnline()
    {
        base.BroadcastMessageAsync(MessageIAmServer, OnMessageSent);
    }

    void OnMessageSent()
    {
        CoroutineUtils.WaitForSeconds(TimeDelaySendServerIsOnlineInSeconds, SendServerOnline);
    }
}

