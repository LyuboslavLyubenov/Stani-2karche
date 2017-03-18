namespace DTOs
{

    using System;

    [Serializable]
    public class ServerInfo_DTO
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
