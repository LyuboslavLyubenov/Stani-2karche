﻿using System;

namespace Assets.Scripts.Network
{

    [Serializable]
    public class ServerInfo_Serializable
    {
        public string ExternalIpAddress;
        public string LocalIPAddress;
        public int ConnectedClientsCount;
        public int MaxConnectionsAllowed;

        public bool IsFull
        {
            get
            {
                return this.ConnectedClientsCount >= this.MaxConnectionsAllowed;
            }
        }
    }

}
