public class LANServerOnlineBroadcastService : LANBroadcastService
{
    public const string MessageIAmServer = "Stani2karcheIAmServer";

    const float TimeDelaySendServerIsOnlineInSeconds = 0.5f;

    public void Initialize()
    {
        base.Initialize();
        SendServerOnline();
    }

    void SendServerOnline()
    {
        base.BroadcastMessageAsync(MessageIAmServer, OnSentMessage);
    }

    void OnSentMessage()
    {
        CoroutineUtils.WaitForSeconds(TimeDelaySendServerIsOnlineInSeconds, SendServerOnline);
    }
}

