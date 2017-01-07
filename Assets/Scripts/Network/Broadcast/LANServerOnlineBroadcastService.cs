namespace Assets.Scripts.Network.Broadcast
{
    using Extensions;
    using Utils;

    public class LANServerOnlineBroadcastService : LANBroadcastService
    {
        public const string BroadcastMessage = "Stani2karcheIAmServer";

        private const float TimeDelaySendServerIsOnlineInSeconds = 1f;
        
        private Timer_ExecuteMethodAfterTime timer;
        
        public LANServerOnlineBroadcastService()
        {
            timer = TimerUtils.ExecuteAfter(TimeDelaySendServerIsOnlineInSeconds, SendServerOnline);
        }
        
        private void SendServerOnline()
        {
            base.BroadcastMessageAsync(BroadcastMessage, this.OnMessageSent);
        }

        private void OnMessageSent()
        {
            this.timer.Reset();
        }

        public override void Dispose()
        {
            this.timer.Stop();
            this.timer.Dispose();
            this.timer = null;

            base.Dispose();
        }
    }

}

