﻿namespace Assets.Scripts.Network.Broadcast
{

    using Assets.Scripts.Extensions;
    using Assets.Scripts.Utils;

    public class LANServerOnlineBroadcastService : LANBroadcastService
    {
        public const string BroadcastMessage = "Stani2karcheIAmServer";

        private const float TimeDelaySendServerIsOnlineInSeconds = 1f;
        
        private Timer_ExecuteMethodAfterTime timer;
        
        public LANServerOnlineBroadcastService()
        {
            this.SendServerOnline();

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
    }

}

