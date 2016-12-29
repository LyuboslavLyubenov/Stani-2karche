namespace Assets.Scripts.Network.TcpSockets
{

    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;

    public class ReceiveMessageState
    {
        public byte[] Buffer = new byte[1024];
        public bool IsReceivedDataSize = false;
        public int DataSizeNeeded = -1;
        public MemoryStream Data = new MemoryStream();
        public Socket Socket = null;

        public string IPAddress
        {
            get;
            private set;
        }

        public ReceiveMessageState(Socket socket)
        {
            this.Socket = socket;

            var ipEndPoint = (IPEndPoint)socket.RemoteEndPoint;
            this.IPAddress = ipEndPoint.Address.ToString().Split(':').First();
        }
    }

}
