using System.Net.Sockets;

namespace Assets.Scripts.Network
{

    public class SendMessageState
    {
        public int DataSentLength = 0;
        public byte[] DataToSend;
        public Socket Client;
    }

}