namespace Utils.Unity
{
    using System;
    using System.Collections;

    using Network.NetworkManagers;

    using UnityEngine;
    using UnityEngine.Networking;
    using UnityEngine.SceneManagement;

    public class NetworkManagerUtils : MonoBehaviour
    {
        private static NetworkManagerUtils instance;

        public static NetworkManagerUtils Instance
        {
            get
            {
                if (instance == null)
                {
                    var obj = new GameObject("NetworkManagerUtils");
                    instance = obj.AddComponent<NetworkManagerUtils>();
                }

                return instance;
            }
        }

        private NetworkManagerUtils()
        {
            SceneManager.activeSceneChanged += this.OnActiveSceneChanged; 
        }

        private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            instance = null;
        }

        /// <summary>
        /// Get ip needed to connect to the server from PlayerPrefsEncryptionUtils. ("ServerLocalIP", "ServerExternalIP") 
        /// If server is available and user internet connection is ok should return ip that can be used to connect to the server. 
        /// Otherwise onError 
        /// </summary>
        public void GetServerIp(Action<string> onFound, Action onError)
        {
            var localIp = PlayerPrefsEncryptionUtils.GetString("ServerLocalIP");
            
            NetworkManagerUtils.Instance.IsServerUp(localIp, ClientNetworkManager.Port, (isRunningLocal) =>
                {
                    if (isRunningLocal)
                    {
                        onFound(localIp);
                        return;
                    }

                    var externalIp = PlayerPrefsEncryptionUtils.GetString("ServerExternalIP");

                    NetworkManagerUtils.Instance.IsServerUp(externalIp, ClientNetworkManager.Port, (isRunningExternal) =>
                        {
                            if (isRunningExternal)
                            {
                                onFound(externalIp);
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

        private IEnumerator IsServerUpCoroutine(string ip, int port, Action<bool> isRunning)
        {
            const int MaxConnectionAttempts = 5;

            var connectionConfig = new ConnectionConfig();
            connectionConfig.MaxConnectionAttempt = MaxConnectionAttempts; 
            
            var topology = new HostTopology(connectionConfig, 2);
            var genericHostId = NetworkTransport.AddHost(topology, 0);

            byte error;
            var connectionId = NetworkTransport.Connect(genericHostId, ip, port, 0, out error);
            
            yield return new WaitForSeconds(1f);

            int recvConnectionId;
            int recvChannelId;
            byte[] buffer = new byte[256];
            int recSize;
            byte recError;
            var eventType = NetworkTransport.ReceiveFromHost(genericHostId, out recvConnectionId, out recvChannelId, buffer, buffer.Length, out recSize, out recError);

            var isUp = eventType == NetworkEventType.ConnectEvent || eventType == NetworkEventType.DisconnectEvent;
            
            yield return new WaitForSeconds(1f);

            byte disconnectError;
            NetworkTransport.Disconnect(genericHostId, connectionId, out disconnectError);
            NetworkTransport.RemoveHost(genericHostId);

            yield return null;
           
            isRunning(isUp);
        }
    }
}
