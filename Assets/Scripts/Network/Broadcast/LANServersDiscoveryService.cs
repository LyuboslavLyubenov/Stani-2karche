namespace Assets.Scripts.Network.Broadcast
{

    using System;

    using EventArgs;

    public class LANServersDiscoveryService : LANBroadcastService
    {
        public EventHandler<IpEventArgs> OnFound = delegate
            {
            };

        private const int RetrieveMessageDelayInSeconds = 1;
        
        // ReSharper disable once ArrangeTypeMemberModifiers
        void Start()
        {
            base.Initialize();
            this.CoroutineUtils.WaitForFrames(1, this.ReceiveIsServerOnlineMessage);
        }

        private void ReceiveIsServerOnlineMessage()
        {
            base.ReceiveBroadcastMessageAsync(this.ReceivedBroadcastMessage);
        }

        private void ReceivedBroadcastMessage(string ip, string message)
        {
            if (message.Equals(LANServerOnlineBroadcastService.MessageIAmServer))
            {
                this.OnFound(this, new IpEventArgs(ip));
            }

            this.CoroutineUtils.WaitForSeconds(RetrieveMessageDelayInSeconds, this.ReceiveIsServerOnlineMessage);
        }
	
    }

}
