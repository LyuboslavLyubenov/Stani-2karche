using System;
using System.Net.Sockets;

namespace Assets.Scripts.Network
{

    public class ClientConnectingState
    {
        public Action OnConnected;
        public Socket Client;
    }

}