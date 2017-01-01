namespace Assets.Scripts.Network.TcpSockets
{
    using System;
    using System.Net.Sockets;

    public class SendMessageState
    {
        public int DataSentLength = 0;
        public byte[] DataToSend;
        public Socket Client;
        public Action OnSent;
        public Action OnError;
    }
}