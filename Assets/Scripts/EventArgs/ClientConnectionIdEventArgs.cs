namespace EventArgs
{

    using System;

    using EventArgs = System.EventArgs;

    public class ClientConnectionIdEventArgs : EventArgs
    {
        public int ConnectionId
        {
            get;
            private set;
        }

        public ClientConnectionIdEventArgs(int connectionId)
        {
            if (connectionId <= 0)
            {
                throw new ArgumentOutOfRangeException("connectionId");
            }

            this.ConnectionId = connectionId;
        }
    }

}