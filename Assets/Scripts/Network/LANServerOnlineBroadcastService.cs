public class LANServerOnlineBroadcastService : LANBroadcastService
{
    public const string MessageIAmServer = "Stani2karcheIAmServer";

    const float TimeDelaySendServerIsOnlineInSeconds = 0.5f;

    public void Start()
    {
        base.Initialize();
        CoroutineUtils.WaitForSeconds(1, SendServerOnline);
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

