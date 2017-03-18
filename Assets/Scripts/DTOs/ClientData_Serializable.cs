namespace DTOs
{

    using System;

    [Serializable]
    public class ClientData_Serializable
    {
        public int ConnectionId;
        public string Username;
    }

    [Serializable]
    public class OnlineClientsData_DTO
    {
        public ClientData_Serializable[] OnlinePlayers;
    }

}