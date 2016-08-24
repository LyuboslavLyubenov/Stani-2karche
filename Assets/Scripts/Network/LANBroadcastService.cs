using System;
using CielaSpike;
using System.Collections;
using System.Security.Cryptography;
using System.Net.Sockets;
using System.Net;
using System.Linq;

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
        udpClient = new UdpClient();
        udpClient.ExclusiveAddressUse = false;
        //enable receiving and sending broadcast
        udpClient.EnableBroadcast = true;
        udpClient.Client.EnableBroadcast = true;
        //lines below basically tell that we gonna receive from/ send to endpoint
        udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        udpClient.Client.Bind(endPoint);
    }

    IEnumerator BroadcastMessageCoroutine(string message, OnSentMessage onSentMessage)
    {
        var messageEncrypted = CipherUtility.Encrypt<RijndaelManaged>(message, ENCRYPTION_PASSWORD, ENCRYPTION_SALT);
        var buffer = System.Text.Encoding.UTF8.GetBytes(messageEncrypted);

        udpClient.Send(buffer, buffer.Length, "255.255.255.255", Port);
        yield return Ninja.JumpToUnity;
        onSentMessage();
    }

    IEnumerator ReceiveMessageCoroutine(OnReceivedMessage onReceivedMessage)
    {
        IPEndPoint receivedEndPoint = null;
        var buffer = udpClient.Receive(ref receivedEndPoint);
        var messageEncrypted = System.Text.Encoding.UTF8.GetString(buffer);
        var messageDecrypted = CipherUtility.Decrypt<RijndaelManaged>(messageEncrypted, ENCRYPTION_PASSWORD, ENCRYPTION_SALT);

        yield return Ninja.JumpToUnity;
        onReceivedMessage(receivedEndPoint.Address.GetIPAddress(), messageDecrypted);
    }

    protected virtual void Initialize()
    {
        var endPoint = listenEndPoint;
        ConfigUDPCLient(endPoint);
    }

    protected virtual void Dispose()
    {
        StopAllCoroutines();
        udpClient.Close();
        udpClient = null;
    }

    protected void BroadcastMessageAsync(string message, OnSentMessage onSentMessage)
    {
        this.StartCoroutineAsync(BroadcastMessageCoroutine(message, onSentMessage));
    }

    protected void BroadcastMessageAsync(string message)
    {
        this.StartCoroutineAsync(
            BroadcastMessageCoroutine(
                message, 
                delegate
                {
                }
            )
        );
    }

    protected void ReceiveBroadcastMessageAsync(OnReceivedMessage onReceivedMessage)
    {
        this.StartCoroutineAsync(ReceiveMessageCoroutine(onReceivedMessage));
    }
}
