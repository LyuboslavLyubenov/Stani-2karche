namespace Assets.Scripts.Network.TcpSockets
{

    using System;
    using System.Net.Sockets;

    public class ClientConnectingState
    {
        public Action OnConnected;
        public Socket Client;
    }

}