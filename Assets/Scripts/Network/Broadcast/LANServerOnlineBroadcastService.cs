namespace Assets.Scripts.Network.Broadcast
{
    public class LANServerOnlineBroadcastService : LANBroadcastService
    {
        public const string MessageIAmServer = "Stani2karcheIAmServer";

        private const float TimeDelaySendServerIsOnlineInSeconds = 1f;

        public void Start()
        {
            base.Initialize();
            this.CoroutineUtils.WaitForSeconds(TimeDelaySendServerIsOnlineInSeconds, this.SendServerOnline);
        }

        private void SendServerOnline()
        {
            base.BroadcastMessageAsync(MessageIAmServer, this.OnMessageSent);
        }

        private void OnMessageSent()
        {
            this.CoroutineUtils.WaitForSeconds(TimeDelaySendServerIsOnlineInSeconds, this.SendServerOnline);
        }
    }

}

