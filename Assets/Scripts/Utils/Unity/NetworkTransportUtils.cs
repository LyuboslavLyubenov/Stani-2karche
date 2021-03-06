﻿namespace Utils.Unity
{

    using System;

    using DTOs;

    using Exceptions;

    using SecuritySettings;

    using UnityEngine;
    using UnityEngine.Networking;
    using UnityEngine.SceneManagement;

    public class NetworkTransportUtils
    {
        private NetworkTransportUtils()
        {
        }

        private static bool IsValidNetworkOperation(byte error)
        {
            const byte errorCodeOk = (byte)NetworkError.Ok; 
            return (error == errorCodeOk);
        }

        private static string ConvertBufferToString(byte[] buffer)
        {
            const char empty = '\0';

            var message = System.Text.Encoding.UTF8.GetString(buffer).TrimEnd(empty);
            return message;
        }

        private static void OnDecryptedMessage(int connectionId, NetworkEventType receiveEventType, string decryptedMessage, Action<NetworkData> onReceivedMessage)
        {
            var networkData = new NetworkData(connectionId, decryptedMessage, receiveEventType);
            onReceivedMessage(networkData);
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
            int bufferSize = 8192;
            byte[] recBuffer = new byte[bufferSize];
            int dataSize;
            byte error;

            NetworkEventType receiveEventType = 
                NetworkTransport.Receive(
                    out recHostId, 
                    out connectionId, 
                    out channelId, 
                    recBuffer, 
                    bufferSize, 
                    out dataSize, 
                    out error);

            if (!IsValidNetworkOperation(error))
            {
                if (onError != null)
                {
                    onError(new Exceptions.NetworkException(error));
                }

                return;
            }

            var message = ConvertBufferToString(recBuffer);
            var networkData = new NetworkData(connectionId, message, receiveEventType);
            onReceivedMessage(networkData);
        }

        public static void SendMessageAsync(int hostId, int connectionId, int channelId, string message, Action<NetworkException> onError = null, Action onSent = null)
        {
            byte error;

            var buffer = System.Text.Encoding.UTF8.GetBytes(message);
            var isSent = NetworkTransport.Send(hostId, connectionId, channelId, buffer, buffer.Length, out error);

            if (!IsValidNetworkOperation(error) || !isSent)
            {
                if (onError != null)
                {
                    onError(new NetworkException(error));
                }

                return;
            }

            if (onSent != null)
            {
                onSent();
            }
        }
    }

    public class NetworkTransportUtilsDummyClass : MonoBehaviour
    {
        public static NetworkTransportUtilsDummyClass Instance
        {
            get
            {
                if (instance == null)
                {
                    var obj = new GameObject();
                    obj.name = "NetworkTransportUtilsDummyClass";
                    instance = obj.AddComponent<NetworkTransportUtilsDummyClass>();
                }

                return instance;
            }
        }

        private static NetworkTransportUtilsDummyClass instance = null;

        // ReSharper disable once ArrangeTypeMemberModifiers
        void Start()
        {
            SceneManager.activeSceneChanged += this.OnActiveSceneChanged;
        }

        private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            SceneManager.activeSceneChanged -= this.OnActiveSceneChanged;
            instance = null;
        }
    }
}