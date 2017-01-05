using System;

namespace Assets.Scripts.EventArgs
{

    using Assets.Scripts.Extensions;

    using EventArgs = System.EventArgs;

    public class IpEventArgs : EventArgs
    {
        public IpEventArgs(string ip)
        {
            if (!ip.IsValidIPV4())
            {
                throw new ArgumentException("Invalid ipv4 address");  
            }

            this.IPAddress = ip;
        }

        public string IPAddress
        {
            get;
            private set;
        }
    }

}