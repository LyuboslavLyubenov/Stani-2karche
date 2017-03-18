namespace Network.TcpSockets
{

    using System.Net.Sockets;

    public class DisconnectState : AsyncOperationStatusCallbacks
    {
        public string IPAddress;
        public Socket Socket;
    }

}