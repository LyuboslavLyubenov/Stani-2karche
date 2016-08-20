using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Linq;

public class ReceiveMessageState
{
    public string EndOfMessage = "[EndOfMessage]";
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
        IPAddress = ipEndPoint.Address.ToString().Split(':').First();
    }
}
