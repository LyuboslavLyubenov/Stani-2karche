using GameInfoEventArgs = EventArgs.GameInfoEventArgs;

namespace Assets.Scripts.Commands.CreatedGameInfoReceiver
{
    using System;

    using Assets.Scripts.Extensions;

    public class ReceiveGameInfoRequest
    {
        public string Ip { get; private set; }

        public Action<GameInfoEventArgs> OnSuccess { get; private set; }
        
        public ReceiveGameInfoRequest(string ip, Action<GameInfoEventArgs> onSuccess)
        {
            if (string.IsNullOrEmpty(ip))
            {
                throw new ArgumentNullException();
            }

            if (!ip.IsValidIPV4())
            {
                throw new ArgumentException("Invalid ip address");
            }

            if (onSuccess == null)
            {
                throw new ArgumentNullException("onSuccess");
            }

            this.Ip = ip;
            this.OnSuccess = onSuccess;
        }

        public override int GetHashCode()
        {
            var hash = 13;
            hash += (hash * 7) + this.Ip.GetHashCode();
            hash += (hash * 7) + this.OnSuccess.GetHashCode();
            return hash;
        }
    }
}