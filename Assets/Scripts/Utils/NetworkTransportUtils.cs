using UnityEngine.Networking;
using System;
using System.Security.Cryptography;
using System.Collections;
using CielaSpike;
using UnityEngine;

public class NetworkTransportUtils
{
    private NetworkTransportUtils()
    {
        
    }

    public static NetworkData ReceiveMessage()
    {
        int recHostId; 
        int connectionId; 
        int channelId; 
        byte[] recBuffer = new byte[1024]; 
        int bufferSize = 1024;
        int dataSize;
        byte error;

        NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);
        ValidateNetworkOperation(error);

        var message = ConvertBufferToString(recBuffer);
      
        if (!string.IsNullOrEmpty(message))
        {
            var decryptedMessage = CipherUtility.Decrypt<RijndaelManaged>(message, SecuritySettings.NETWORK_ENCRYPTION_PASSWORD, SecuritySettings.SALT); 
            message = decryptedMessage;
        }

        return new NetworkData(connectionId, message, recData);
    }

    public static void ReceiveMessageAsync(Action<NetworkData> onReceivedMessage, Action<Exception> onError)
    {
        if (onReceivedMessage == null)
        {
            throw new ArgumentNullException("onReceivedMessage");
        }

        if (onError == null)
        {
            throw new ArgumentNullException("onError");
        }

        NetworkTransportUtilsDummyClass.Instance.StartCoroutineAsync(ReceiveMessageAsyncCoroutine(onReceivedMessage, onError));
    }

    static IEnumerator ReceiveMessageAsyncCoroutine(Action<NetworkData> onReceivedMessage, Action<Exception> onError)
    {
        yield return null;

        NetworkData networkData = new NetworkData(-1, string.Empty, NetworkEventType.Nothing);
        Exception exception = null;

        try
        {
            networkData = ReceiveMessage();    
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        if (exception != null)
        {
            yield return Ninja.JumpToUnity;
            onError(exception);
            yield break;
        }

        yield return Ninja.JumpToUnity;
        onReceivedMessage(networkData);
    }

    public static void SendMessage(int hostId, int connectionId, int channelId, string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            throw new ArgumentNullException("message");
        }

        var encryptedMessage = CipherUtility.Encrypt<RijndaelManaged>(message, SecuritySettings.NETWORK_ENCRYPTION_PASSWORD, SecuritySettings.SALT);
        var buffer = System.Text.Encoding.UTF8.GetBytes(encryptedMessage);
        byte error;

        NetworkTransport.Send(hostId, connectionId, channelId, buffer, buffer.Length, out error);
        ValidateNetworkOperation(error);
    }

    public static void SendMessageAsync(int hostId, int connectionId, int channelId, string message, Action<Exception> onError = null)
    {
        NetworkTransportUtilsDummyClass.Instance.StartCoroutineAsync(SendMessageAsyncCoroutine(hostId, connectionId, channelId, message, onError));
    }

    static IEnumerator SendMessageAsyncCoroutine(int hostId, int connectionId, int channelId, string message, Action<Exception> onError = null)
    {
        yield return null;

        Exception exception = null;

        try
        {
            SendMessage(hostId, connectionId, channelId, message);
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        if (exception != null)
        {
            if (onError != null)
            {
                onError(exception);
            }

            yield break;
        }
    }

    static void ValidateNetworkOperation(byte error)
    {
        var networkError = (NetworkError)error;
        if (networkError != NetworkError.Ok)
        {
            throw new NetworkException(error);
        }
    }

    static string ConvertBufferToString(byte[] buffer)
    {
        var message = System.Text.Encoding.UTF8.GetString(buffer).ToCharArray();
        var result = new System.Text.StringBuilder();

        foreach (var c in message)
        {
            if (c != '\0')
            {
                result.Append(c);
            }
        }

        return result.ToString();
    }
}

public class NetworkTransportUtilsDummyClass : MonoBehaviour
{
    public static NetworkTransportUtilsDummyClass Instance
    {
        get
        {
            if (_instance == null)
            {
                var obj = new GameObject();
                obj.name = "NetworkTransportUtilsDummyClass";
                _instance = obj.AddComponent<NetworkTransportUtilsDummyClass>();
            }

            return _instance;
        }
    }

    static NetworkTransportUtilsDummyClass _instance = null;
}