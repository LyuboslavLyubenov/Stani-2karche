namespace Assets.Scripts.Utils.Unity
{

    using System;
    using System.Collections;

    using Assets.Scripts.Network;
    using Assets.Scripts.Network.NetworkManagers;

    using UnityEngine;
    using UnityEngine.Networking;

    public class NetworkManagerUtils : MonoBehaviour
    {
        static NetworkManagerUtils instance;

        public static NetworkManagerUtils Instance
        {
            get
            {
                if (instance == null)
                {
                    var obj = new GameObject();
                    obj.name = "NetworkManagerUtils";
                    instance = obj.AddComponent<NetworkManagerUtils>();
                }

                return instance;
            }
        }

        NetworkManagerUtils()
        {
        
        }

        /// <summary>
        /// Get ip needed to connect to the server from PlayerPrefsEncryptionUtils. 
        /// If server is available and user internet connection is ok should return ip that can be used to connect to the server. 
        /// Otherwise onError 
        /// </summary>
        public void GetServerIp(Action<string> onFound, Action onError)
        {
            var localIp = PlayerPrefsEncryptionUtils.GetString("ServerLocalIP");
            var externalIp = PlayerPrefsEncryptionUtils.HasKey("ServerExternalIP") ? PlayerPrefsEncryptionUtils.GetString("ServerExternalIP") : localIp;

            NetworkManagerUtils.Instance.IsServerUp(externalIp, ClientNetworkManager.Port, (isRunningExternal) =>
                {

                    if (isRunningExternal)
                    {
                        onFound(externalIp);
                        return;
                    }

                    NetworkManagerUtils.Instance.IsServerUp(localIp, ClientNetworkManager.Port, (isRunningLocal) =>
                        {
                            if (isRunningLocal)
                            {
                                onFound(localIp);
                                return;
                            }

                            onError();
                        });    
                });
        }

        public void IsServerUp(string ip, int port, Action<bool> isUp)
        {
            this.StartCoroutine(this.IsServerUpCoroutine(ip, port, isUp));
        }

        IEnumerator IsServerUpCoroutine(string ip, int port, Action<bool> isRunning)
        {
            const int MaxConnectionAttempts = 5;

            var connectionConfig = new ConnectionConfig();
            connectionConfig.MaxConnectionAttempt = MaxConnectionAttempts; 

            var communicationChannel = connectionConfig.AddChannel(QosType.ReliableSequenced);
            var topology = new HostTopology(connectionConfig, 2);
            var genericHostId = NetworkTransport.AddHost(topology, 0);

            byte error;
            var connectionId = NetworkTransport.Connect(genericHostId, ip, port, 0, out error);

            var networkError = (NetworkConnectionError)error;
            bool isUp = false;

            yield return new WaitForSeconds(1f);

            int recvConnectionId;
            int recvChannelId;
            byte[] buffer = new byte[512];
            int recSize;
            byte recError;
            var eventType = NetworkTransport.ReceiveFromHost(genericHostId, out recvConnectionId, out recvChannelId, buffer, buffer.Length, out recSize, out recError);

            isUp = eventType == NetworkEventType.ConnectEvent;

            yield return new WaitForEndOfFrame();

            byte disconnectError;
            NetworkTransport.Disconnect(genericHostId, connectionId, out disconnectError);
            NetworkTransport.RemoveHost(genericHostId);

            yield return new WaitForEndOfFrame();

            isRunning(isUp);
        }

    }

}
