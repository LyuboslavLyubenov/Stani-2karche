using System;

using UnityEngine.Networking;

namespace Assets.Scripts.Network
{

    public class NetworkData
    {
        public NetworkData(int connectionId, string message, NetworkEventType networkEventType)
        {
            if (connectionId < 0)
            {
                throw new ArgumentOutOfRangeException("connectionId");
            }

            this.ConnectionId = connectionId;
            this.Message = message;
            this.NetworkEventType = networkEventType;
        }

        public int ConnectionId
        {
            get;
            private set;
        }

        public string Message
        {
            get;
            set;
        }

        public NetworkEventType NetworkEventType
        {
            get;
            private set;
        }
    }

}
