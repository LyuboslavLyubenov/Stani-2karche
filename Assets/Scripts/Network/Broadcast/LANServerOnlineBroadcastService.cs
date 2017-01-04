namespace Assets.Scripts.Network.Broadcast
{

    using Assets.Scripts.Utils;

    public class LANServerOnlineBroadcastService : LANBroadcastService
    {
        public const string MessageIAmServer = "Stani2karcheIAmServer";

        private const float TimeDelaySendServerIsOnlineInSeconds = 1f;

        public LANServerOnlineBroadcastService()
        {
            this.SendServerOnline();
        }

        private void StartTimer()
        {
            var timer = TimerUtils.ExecuteAfter(TimeDelaySendServerIsOnlineInSeconds, this.OnMessageSent);

            timer.RunOnUnityThread = true;
            timer.AutoDispose = true;
        }

        private void SendServerOnline()
        {
            StartTimer();
            base.BroadcastMessageAsync(MessageIAmServer, this.OnMessageSent);
        }

        private void OnMessageSent()
        {
            StartTimer();
        }
    }

}

