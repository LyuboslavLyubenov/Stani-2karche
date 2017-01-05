namespace Assets.Scripts.Network.Broadcast
{
    using System;

    using Assets.Scripts.Extensions;

    using Utils;
    using EventArgs;

    public class LANServersDiscoveryService : LANBroadcastService
    {
        private const int RetrieveMessageDelayInSeconds = 1;

        public EventHandler<IpEventArgs> OnFound = delegate
            {
            };

        private Timer_ExecuteMethodAfterTime timer;
        
        public LANServersDiscoveryService()
        {
            this.timer = TimerUtils.ExecuteAfter(RetrieveMessageDelayInSeconds, this.ReceiveIsServerOnlineMessage);
            this.timer.RunOnUnityThread = true;
            this.timer.Start();
        }

        private void ReceiveIsServerOnlineMessage()
        {
            base.ReceiveBroadcastMessageAsync(this.ReceivedBroadcastMessage);
        }

        private void ReceivedBroadcastMessage(string ip, string message)
        {
            if (message.Equals(LANServerOnlineBroadcastService.BroadcastMessage))
            {
                this.OnFound(this, new IpEventArgs(ip));
            }
            
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
