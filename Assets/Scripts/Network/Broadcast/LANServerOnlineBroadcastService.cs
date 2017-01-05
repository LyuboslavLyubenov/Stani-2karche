namespace Assets.Scripts.Network.Broadcast
{

    using Assets.Scripts.Utils;

    public class LANServerOnlineBroadcastService : LANBroadcastService
    {
        public const string BroadcastMessage = "Stani2karcheIAmServer";

        private const float TimeDelaySendServerIsOnlineInSeconds = 1f;
        
        private Timer_ExecuteMethodAfterTime timer;
        
        public LANServerOnlineBroadcastService()
        {
            this.SendServerOnline();

            timer = TimerUtils.ExecuteAfter(TimeDelaySendServerIsOnlineInSeconds, this.OnMessageSent);
            timer.RunOnUnityThread = true;
        }
        
        private void SendServerOnline()
        {
            base.BroadcastMessageAsync(BroadcastMessage, this.OnMessageSent);
        }

        private void OnMessageSent()
        {
            this.timer.Stop();
            this.timer.Start();
        }
    }

}

