namespace Network.Broadcast
{

    using System;

    using EventArgs;

    using Extensions;

    using Interfaces.Services;

    using Utils;

    public class LANServersDiscoverer : LANBroadcastService, ILANServersDiscoverer
    {
        private const int RetrieveMessageDelayInSeconds = 1;

        public event EventHandler<IpEventArgs> OnFound = delegate
            {
            };

        private Timer_ExecuteMethodAfterTime timer;
        
        public LANServersDiscoverer()
        {
            var threadUtils = ThreadUtils.Instance;

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
