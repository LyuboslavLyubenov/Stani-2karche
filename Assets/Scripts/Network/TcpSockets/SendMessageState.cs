namespace Network.TcpSockets
{

    using System.Net.Sockets;

    public class SendMessageState : AsyncOperationStatusCallbacks
    {
        public int DataSentLength = 0;
        public byte[] DataToSend;
        public Socket Client;
    }
}