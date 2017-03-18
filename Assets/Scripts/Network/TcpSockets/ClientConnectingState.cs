namespace Network.TcpSockets
{

    using System.Net.Sockets;

    public class ClientConnectingState : AsyncOperationStatusCallbacks
    {
        public Socket Client;
    }

}