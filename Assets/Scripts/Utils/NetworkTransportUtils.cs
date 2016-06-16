using UnityEngine.Networking;
using System;
using System.Security.Cryptography;

public class NetworkTransportUtils
{
    public static NetworkData RecieveMessage()
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

