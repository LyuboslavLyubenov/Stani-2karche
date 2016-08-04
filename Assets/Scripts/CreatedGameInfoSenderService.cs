using UnityEngine;
using System.Collections;
using System;
using System.Net.Sockets;
using CielaSpike;
using System.Text;
using System.Threading;
using System.Security.Cryptography;

public class CreatedGameInfoSenderService : ExtendedMonoBehaviour
{
    const int Port = 7773;

    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    bool sent = false;

    public bool Sent
    {
        get
        {
            return sent;
        }
    }

    void Start()
    {
        socket.ExclusiveAddressUse = false;
    }

    void ConnectTo(string ip)
    {
        if (!ip.IsValidIPV4())
        {
            throw new ArgumentException("Invalid ipv4 address");
        }

        socket.Connect(ip, Port);
    }

    void _SendGameInfo(string ip, CreatedGameInfo_Serializable gameInfo)
    {
        const string GameInfoTag = "[CreatedGameInfo]";

        try
        {
            if (!socket.Connected)
            {
                ConnectTo(ip);    
            }

            var gameInfoJSON = GameInfoTag + JsonUtility.ToJson(gameInfo);
            var encrypted = CipherUtility.Encrypt<RijndaelManaged>(gameInfoJSON, SimpleTcpServer.ENCRYPTION_PASSWORD, SimpleTcpServer.ENCRYPTION_SALT);
            var buffer = Encoding.UTF8.GetBytes(gameInfoJSON);
            socket.Send(buffer);
            sent = true;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    IEnumerator SendGameInfoCoroutine(string ip, CreatedGameInfo_Serializable gameInfo, Action OnSent)
    {
        Thread.Sleep(100);

        sent = false;
        _SendGameInfo(ip, gameInfo);

        if (!sent)
        {
            yield return Ninja.JumpToUnity;
            CoroutineUtils.WaitForSeconds(1, () => SendGameInfo(ip, gameInfo, OnSent));
        }
        else
        {
            OnSent.Invoke();
        }
    }

    public void SendGameInfo(string ip, CreatedGameInfo_Serializable gameInfo, Action OnSent)
    {
        this.StartCoroutineAsync(SendGameInfoCoroutine(ip, gameInfo, OnSent));
    }
}
