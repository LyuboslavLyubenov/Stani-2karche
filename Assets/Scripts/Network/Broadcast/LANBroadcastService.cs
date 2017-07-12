namespace Network.Broadcast
{

    using System;
    using System.Collections;
    using System.Net;
    using System.Net.Sockets;
    using System.Security.Cryptography;

    using CielaSpike.Thread_Ninja;

    using Extensions;

    using SecuritySettings;

    using Utils;

    public abstract class LANBroadcastService : IDisposable
    {
        private const int Port = 7771;

        public delegate void OnReceivedMessage(string ip, string message);
        public delegate void OnSentMessage();

        private UdpClient udpClient = null;

        private IPEndPoint listenEndPoint = new IPEndPoint(IPAddress.Any, Port);

        protected LANBroadcastService()
        {
            var endPoint = this.listenEndPoint;
            this.ConfigUDPCLient(endPoint);
        }

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

        private IEnumerator BroadcastMessageCoroutine(string message, OnSentMessage onSentMessage = null)
        {
            var messageEncrypted = CipherUtility.Encrypt<RijndaelManaged>(message, SecuritySettings.LANBROADCASTSERVICE_PASSWORD, SecuritySettings.SALT);
            var buffer = System.Text.Encoding.UTF8.GetBytes(messageEncrypted);
            var ip = IPAddress.Broadcast.GetIPAddress();
            this.udpClient.Send(buffer, buffer.Length, ip, Port);

            yield return Ninja.JumpToUnity;

            if (onSentMessage != null)
            {
                onSentMessage();
            }
        }

        private IEnumerator ReceiveMessageCoroutine(OnReceivedMessage onReceivedMessage)
        {
            IPEndPoint receivedEndPoint = null;
            var buffer = this.udpClient.Receive(ref receivedEndPoint);
            var messageEncrypted = System.Text.Encoding.UTF8.GetString(buffer);
            var messageDecrypted = CipherUtility.Decrypt<RijndaelManaged>(messageEncrypted, SecuritySettings.LANBROADCASTSERVICE_PASSWORD, SecuritySettings.SALT);

            yield return Ninja.JumpToUnity;

            onReceivedMessage(receivedEndPoint.Address.GetIPAddress(), messageDecrypted);
        }

        protected void BroadcastMessageAsync(string message, OnSentMessage onSentMessage = null)
        {
            ThreadUtils.Instance.RunOnBackgroundThread(this.BroadcastMessageCoroutine(message, onSentMessage));
        }

        protected void ReceiveBroadcastMessageAsync(OnReceivedMessage onReceivedMessage)
        {
            ThreadUtils.Instance.RunOnBackgroundThread(this.ReceiveMessageCoroutine(onReceivedMessage));
        }
        
        public virtual void Dispose()
        {
            this.udpClient.Close();
            this.udpClient = null;
        }
    }
}