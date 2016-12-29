namespace Assets.Scripts.Network.Broadcast
{

    using System.Collections;
    using System.Net;
    using System.Net.Sockets;
    using System.Security.Cryptography;

    using Assets.CielaSpike.Thread_Ninja;
    using Assets.Scripts.Extensions;
    using Assets.Scripts.Utils;
    using Assets.Scripts.Utils.Unity;

    public abstract class LANBroadcastService : ExtendedMonoBehaviour
    {
        const int Port = 7771;
        const string ENCRYPTION_PASSWORD = "72a23c2e4152b09ca0b3cf2563c85eb2";
        const string ENCRYPTION_SALT = "21a87b0b0eb48a341889bf1cb818db67";

        public delegate void OnReceivedMessage(string ip,string message);

        public delegate void OnSentMessage();

        UdpClient udpClient = null;
        IPEndPoint listenEndPoint = new IPEndPoint(IPAddress.Any, Port);

        void ConfigUDPCLient(IPEndPoint endPoint)
        {
            this.udpClient = new UdpClient();
            this.udpClient.ExclusiveAddressUse = false;
            //enable receiving and sending broadcast
            this.udpClient.EnableBroadcast = true;
            //lines below basically tell that we gonna receive from/ send to endpoint
            this.udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            this.udpClient.Client.Bind(endPoint);
        }

        IEnumerator BroadcastMessageCoroutine(string message, OnSentMessage onSentMessage)
        {
            var messageEncrypted = CipherUtility.Encrypt<RijndaelManaged>(message, ENCRYPTION_PASSWORD, ENCRYPTION_SALT);
            var buffer = System.Text.Encoding.UTF8.GetBytes(messageEncrypted);
            var ip = IPAddress.Broadcast.GetIPAddress();
            this.udpClient.Send(buffer, buffer.Length, ip, Port);
            yield return Ninja.JumpToUnity;
            onSentMessage();

            UnityEngine.Debug.Log("LANBroadcast BroadcastMessage " + message);
        }

        IEnumerator ReceiveMessageCoroutine(OnReceivedMessage onReceivedMessage)
        {
            IPEndPoint receivedEndPoint = null;
            var buffer = this.udpClient.Receive(ref receivedEndPoint);
            var messageEncrypted = System.Text.Encoding.UTF8.GetString(buffer);
            var messageDecrypted = CipherUtility.Decrypt<RijndaelManaged>(messageEncrypted, ENCRYPTION_PASSWORD, ENCRYPTION_SALT);

            yield return Ninja.JumpToUnity;
            onReceivedMessage(receivedEndPoint.Address.GetIPAddress(), messageDecrypted);

            UnityEngine.Debug.Log("LANBroadcast ReceivedMessageAsync - message " + messageDecrypted);
        }

        protected virtual void Initialize()
        {
            var endPoint = this.listenEndPoint;
            this.ConfigUDPCLient(endPoint);

            UnityEngine.Debug.Log("LANBroadcast intiialized");
        }

        protected virtual void Dispose()
        {
            this.StopAllCoroutines();
            this.udpClient.Close();
            this.udpClient = null;
            UnityEngine.Debug.Log("LANBroadcast disposed");
        }

        protected void BroadcastMessageAsync(string message, OnSentMessage onSentMessage)
        {
            this.StartCoroutineAsync(this.BroadcastMessageCoroutine(message, onSentMessage));
        }

        protected void BroadcastMessageAsync(string message)
        {
            this.StartCoroutineAsync(
                this.BroadcastMessageCoroutine(
                    message, 
                    delegate
                        {
                        }
                    )
                );
        }

        protected void ReceiveBroadcastMessageAsync(OnReceivedMessage onReceivedMessage)
        {
            this.StartCoroutineAsync(this.ReceiveMessageCoroutine(onReceivedMessage));
        }
    }

}
