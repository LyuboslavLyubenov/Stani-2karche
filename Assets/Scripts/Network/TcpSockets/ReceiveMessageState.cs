namespace Network.TcpSockets
{

    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;

    public class ReceiveMessageState
    {
        private int dataSizeNeeded;

        public byte[] Buffer
        {
            get;
            private set;
        }

        public bool IsReceivedDataSize
        {
            get;
            private set;
        }

        public int DataSizeNeeded
        {
            get
            {
                return this.dataSizeNeeded;
            }
            set
            {
                this.dataSizeNeeded = value;
                this.IsReceivedDataSize = true;
            }
        }

        public MemoryStream Data
        {
            get;
            private set;
        }

        public Socket Socket
        {
            get;
            private set;
        }

        public string IPAddress
        {
            get;
            private set;
        }

        

        public ReceiveMessageState(Socket socket, int buffer)
        {
            this.Socket = socket;
            this.Buffer = new byte[buffer];
            this.Data = new MemoryStream();

            var ipEndPoint = (IPEndPoint)socket.RemoteEndPoint;
            this.IPAddress = ipEndPoint.Address.ToString().Split(':').First();
        }
    }
}
