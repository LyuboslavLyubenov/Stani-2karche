namespace Assets.Scripts.Network.Broadcast
{

    using System.Collections;
    using System.Net;
    using System.Net.Sockets;
    using System.Security.Cryptography;

    using CielaSpike.Thread_Ninja;
    using Extensions;
    using Utils;
    using Utils.Unity;
    using SecuritySettings;

    public abstract class LANBroadcastService : ExtendedMonoBehaviour
    {
        private const int Port = 7771;
        
        public delegate void OnReceivedMessage(string ip,string message);

        public delegate void OnSentMessage();

        private UdpClient udpClient = null;

        private IPEndPoint listenEndPoint = new IPEndPoint(IPAddress.Any, Port);

        private void ConfigUDPCLient(IPEndPoint endPoint)
        {
            this.udpClient = new UdpClient();
            this.udpClient.ExclusiveAddressUse = false;
            //enable receiving and sending broadcast
            this.udpClient.EnableBroadcast = true;
            //lines below basically tell that we gonna receive from/ send to endpoint
            this.udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            this.udpClient.Client.Bind(endPoint);
        }

        private IEnumerator BroadcastMessageCoroutine(string message, OnSentMessage onSentMessage)
        {
            var messageEncrypted = CipherUtility.Encrypt<RijndaelManaged>(message, SecuritySettings.LANBROADCASTSERVICE_PASSWORD, SecuritySettings.SALT);
            var buffer = System.Text.Encoding.UTF8.GetBytes(messageEncrypted);
            var ip = IPAddress.Broadcast.GetIPAddress();
            this.udpClient.Send(buffer, buffer.Length, ip, Port);
            yield return Ninja.JumpToUnity;
            onSentMessage();

            UnityEngine.Debug.Log("LANBroadcast BroadcastMessage " + message);
        }

        private IEnumerator ReceiveMessageCoroutine(OnReceivedMessage onReceivedMessage)
        {
            IPEndPoint receivedEndPoint = null;
            var buffer = this.udpClient.Receive(ref receivedEndPoint);
            var messageEncrypted = System.Text.Encoding.UTF8.GetString(buffer);
            var messageDecrypted = CipherUtility.Decrypt<RijndaelManaged>(messageEncrypted, SecuritySettings.LANBROADCASTSERVICE_PASSWORD, SecuritySettings.SALT);

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
