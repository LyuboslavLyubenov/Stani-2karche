namespace Assets.Scripts.Network.TcpSockets
{

    using System;
    using System.Net.Sockets;

    using Assets.Scripts.Utils;

    public class DisconnectState
    {
        public string IPAddress;
        public Socket Socket;

        public DisconnectState(string IPAddress, Socket socket)
        {
            if (!IPAddress.IsValidIPV4())
            {
                throw new ArgumentException("Invalid ipv4 address", "IPAddress");
            }

            if (socket == null)
            {
                throw new NullReferenceException("Socket cannot be null");
            }

            this.IPAddress = IPAddress;
            this.Socket = socket;
        }
    }

}