namespace Assets.Scripts.Network.Broadcast
{

    using System;

    using Assets.Scripts.EventArgs;

    public class LANServersDiscoveryService : LANBroadcastService
    {
        public EventHandler<IpEventArgs> OnFound = delegate
            {
            };

        const int RetrieveMessageDelayInSeconds = 1;

        void Start()
        {
            base.Initialize();
            this.CoroutineUtils.WaitForFrames(1, this.ReceiveIsServerOnlineMessage);
        }

        void ReceiveIsServerOnlineMessage()
        {
            base.ReceiveBroadcastMessageAsync(this.ReceivedBroadcastMessage);
        }

        void ReceivedBroadcastMessage(string ip, string message)
        {
            if (message.Equals(LANServerOnlineBroadcastService.MessageIAmServer))
            {
                this.OnFound(this, new IpEventArgs(ip));
            }

            this.CoroutineUtils.WaitForSeconds(RetrieveMessageDelayInSeconds, this.ReceiveIsServerOnlineMessage);
        }
	
    }

}
