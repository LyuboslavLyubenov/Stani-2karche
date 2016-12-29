namespace Assets.Scripts.Network.Broadcast
{

    public class LANServerOnlineBroadcastService : LANBroadcastService
    {
        public const string MessageIAmServer = "Stani2karcheIAmServer";

        const float TimeDelaySendServerIsOnlineInSeconds = 1f;

        public void Start()
        {
            base.Initialize();
            this.CoroutineUtils.WaitForSeconds(TimeDelaySendServerIsOnlineInSeconds, this.SendServerOnline);
        }

        void SendServerOnline()
        {
            base.BroadcastMessageAsync(MessageIAmServer, this.OnMessageSent);
        }

        void OnMessageSent()
        {
            this.CoroutineUtils.WaitForSeconds(TimeDelaySendServerIsOnlineInSeconds, this.SendServerOnline);
        }
    }

}

