﻿namespace Assets.Scripts.Utils.Unity
{
    using System;
    using System.Collections;
    using System.Security.Cryptography;

    using CielaSpike.Thread_Ninja;
    using DTOs;
    using Exceptions;
    using SecuritySettings;

    using UnityEngine;
    using UnityEngine.Networking;

    public class NetworkTransportUtils
    {
        private NetworkTransportUtils()
        {
        
        }

        public static void ReceiveMessageAsync(Action<NetworkData> onReceivedMessage, Action<NetworkException> onError = null)
        {
            if (onReceivedMessage == null)
            {
                throw new ArgumentNullException("onReceivedMessage");
            }

            int recHostId; 
            int connectionId; 
            int channelId; 
            int bufferSize = 4096;
            byte[] recBuffer = new byte[bufferSize]; 
            int dataSize;
            byte error;

            NetworkEventType receiveEventType = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);
            
            try
            {
                ValidateNetworkOperation(error);    
            }
            catch (NetworkException ex)
            {
                if (onError != null)
                {
                    onError(ex);
                    return;
                }

                throw;
            }

            var message = ConvertBufferToString(recBuffer);

            DecryptMessageAsync(message, (decryptedMessage) =>
                {
                    var networkData = new NetworkData(connectionId, decryptedMessage, receiveEventType);
                    onReceivedMessage(networkData);
                });
        }

        private static void DecryptMessageAsync(string message, Action<string> onDecrypted)
        {   
            if (onDecrypted == null)
            {
                throw new ArgumentNullException("onDecrypted");
            }

            if (string.IsNullOrEmpty(message))
            {
                onDecrypted(message);
                return;
            }

            NetworkTransportUtilsDummyClass.Instance.StartCoroutineAsync(DecryptMessageAsyncCoroutine(message, onDecrypted));
        }

        private static IEnumerator DecryptMessageAsyncCoroutine(string message, Action<string> onDecrypted)
        {
            var decryptedMessage = string.Empty;

            if (!string.IsNullOrEmpty(message))
            {
                decryptedMessage = CipherUtility.Decrypt<RijndaelManaged>(message, SecuritySettings.NETWORK_ENCRYPTION_PASSWORD, SecuritySettings.SALT); 
            }

            yield return Ninja.JumpToUnity;
            onDecrypted(decryptedMessage);
        }

        public static void SendMessageAsync(int hostId, int connectionId, int channelId, string message, Action<NetworkException> onError = null)
        {
            EncryptMessageAsync(message, (buffer) =>
                {
                    byte error;

                    NetworkTransport.Send(hostId, connectionId, channelId, buffer, buffer.Length, out error);

                    try
                    {
                        ValidateNetworkOperation(error);    
                    }
                    catch (NetworkException ex)
                    {
                        if (onError != null)
                        {
                            onError(ex);
                            return;
                        }

                        throw;
                    }
                });
        }

        private static void EncryptMessageAsync(string message, Action<byte[]> onEncrypted)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentException("Message cannot be empty");
            }

            if (onEncrypted == null)
            {
                throw new ArgumentNullException("onEncrypted");
            }

            NetworkTransportUtilsDummyClass.Instance.StartCoroutineAsync(EncryptMessageAsyncCoroutine(message, onEncrypted));
        }

        private static IEnumerator EncryptMessageAsyncCoroutine(string message, Action<byte[]> onEncrypted)
        {
            var encryptedMessage = CipherUtility.Encrypt<RijndaelManaged>(message, SecuritySettings.NETWORK_ENCRYPTION_PASSWORD, SecuritySettings.SALT);
            var buffer = System.Text.Encoding.UTF8.GetBytes(encryptedMessage);

            yield return Ninja.JumpToUnity;

            onEncrypted(buffer);
        }

        private static void ValidateNetworkOperation(byte error)
        {
            var networkError = (NetworkError)error;
            if (networkError != NetworkError.Ok)
            {
                throw new NetworkException(error);
            }
        }

        private static string ConvertBufferToString(byte[] buffer)
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

        private static NetworkTransportUtilsDummyClass _instance = null;
    }
}