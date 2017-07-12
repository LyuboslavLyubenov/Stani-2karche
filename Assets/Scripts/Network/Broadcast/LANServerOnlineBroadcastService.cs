namespace Network.Broadcast
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
            this.timer = TimerUtils.ExecuteAfter(TimeDelaySendServerIsOnlineInSeconds, this.SendServerOnline);
            this.timer.AutoDispose = false;
            this.timer.RunOnUnityThread = true;
            this.timer.Start();
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