namespace EventArgs
{

    using System;

    using EventArgs = System.EventArgs;

    public class ClientConnectionDataEventArgs : EventArgs
    {
        public int ConnectionId
        {
            get;
            private set;
        }

        public ClientConnectionDataEventArgs(int connectionId)
        {
            if (connectionId <= 0)
            {
                throw new ArgumentOutOfRangeException("connectionId");
            }

            this.ConnectionId = connectionId;
        }
    }

}