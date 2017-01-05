namespace Assets.Scripts.Network.Broadcast
{
    using System;
    using Utils;
    using EventArgs;

    public class LANServersDiscoveryService : LANBroadcastService
    {
        public EventHandler<IpEventArgs> OnFound = delegate
            {
            };

        private const int RetrieveMessageDelayInSeconds = 1;
        

        public LANServersDiscoveryService()
        {
            this.ReceiveIsServerOnlineMessage();
        }
        
        private void ReceiveIsServerOnlineMessage()
        {
            var timer = TimerUtils.ExecuteAfter(RetrieveMessageDelayInSeconds, this.ReceiveIsServerOnlineMessage);

            timer.RunOnUnityThread = true;
            timer.AutoDispose = true;
            timer.Start();

            base.ReceiveBroadcastMessageAsync(this.ReceivedBroadcastMessage);
        }

        private void ReceivedBroadcastMessage(string ip, string message)
        {
            if (message.Equals(LANServerOnlineBroadcastService.BroadcastMessage))
            {
                this.OnFound(this, new IpEventArgs(ip));
            }

            var timer = TimerUtils.ExecuteAfter(RetrieveMessageDelayInSeconds, this.ReceiveIsServerOnlineMessage);

            timer.RunOnUnityThread = true;
            timer.AutoDispose = true;
            timer.Start();
        }
    }
}
