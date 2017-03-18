namespace Controllers
{

    using System;
    using System.Collections.Generic;

    using DTOs;

    using EventArgs;

    using Network.NetworkManagers;

    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    using Utils.Unity;

    public class ConnectedClientsUIController : ExtendedMonoBehaviour
    {
        private const int YOffset = 75;

        private const int DistanceBetweenElements = 5;

        public EventHandler<ClientConnectionDataEventArgs> OnSelectedPlayer = delegate
            {
            };

        public Transform Content;
        public ObjectsPool ConnectedClientsDataPool;

        private float clientElementPrefabHeight;

        private RectTransform contentRectTransform;

        private Dictionary<int, Transform> connectionIdClientElement = new Dictionary<int, Transform>();

        // ReSharper disable once ArrangeTypeMemberModifiers
        void Start()
        {
            var connectedClientDataPrefab = Resources.Load<GameObject>("Prefabs/ConnectedClientElementData");
            var rectTransform = connectedClientDataPrefab.GetComponent<RectTransform>();

            this.clientElementPrefabHeight = rectTransform.sizeDelta.y;
            this.ConnectedClientsDataPool.Prefab = connectedClientDataPrefab.transform;

            this.contentRectTransform = this.Content.GetComponent<RectTransform>();

            ServerNetworkManager.Instance.OnClientConnected += this.OnClientConnected;
            ServerNetworkManager.Instance.OnClientDisconnected += this.OnClientDisconnected;
            ServerNetworkManager.Instance.OnClientSetUsername += this.OnClientSetUsername;
        }

        private void OnClientConnected(object sender, ClientConnectionDataEventArgs args)
        {
            var connectionId = args.ConnectionId;

            if (!this.connectionIdClientElement.ContainsKey(connectionId))
            {
                this.AddConnectedClientToList(connectionId);
                this.UpdateContainerSize();
            }
        }

        private void OnClientDisconnected(object sender, ClientConnectionDataEventArgs args)
        {
            if (this.connectionIdClientElement.ContainsKey(args.ConnectionId))
            {
                var connectionId = args.ConnectionId;
                var clientElement = this.connectionIdClientElement[connectionId];
                var button = clientElement.GetComponent<Button>();

                button.onClick.RemoveAllListeners();
                clientElement.gameObject.SetActive(false);

                this.connectionIdClientElement.Remove(args.ConnectionId);

                this.UpdateContainerSize();
            }
        }

        private void OnClientSetUsername(object sender, ConnectedClientDataEventArgs args)
        {
            var connectionId = args.ClientData.ConnectionId;

            if (!this.connectionIdClientElement.ContainsKey(connectionId))
            {
                return;
            }
            
            var controller = this.connectionIdClientElement[connectionId].GetComponent<ConnectedClientDataElementUIController>();
            controller.Fill(args.ClientData);
        }

        private void AddConnectedClientToList(int connectionId)
        {
            var connectedClientElement = this.ConnectedClientsDataPool.Get();
            var rectTransform = connectedClientElement.GetComponent<RectTransform>();
            var newY = YOffset + ((this.clientElementPrefabHeight + DistanceBetweenElements) * this.connectionIdClientElement.Count);
            rectTransform.anchoredPosition = new Vector2(0, -newY);

            var username = ServerNetworkManager.Instance.GetClientUsername(connectionId);
            var clientData = new ConnectedClientData(connectionId, username);
            var connectedClientElementController = connectedClientElement.GetComponent<ConnectedClientDataElementUIController>();
            this.CoroutineUtils.WaitForFrames(1, () => connectedClientElementController.Fill(clientData));
        
            var button = connectedClientElement.transform.GetComponent<Button>();
            button.onClick.AddListener(this.ClickedOnPlayer);

            this.connectionIdClientElement.Add(connectionId, connectedClientElement);
        }

        private void ClickedOnPlayer()
        {
            var obj = EventSystem.current.currentSelectedGameObject;
            var controller = obj.GetComponent<ConnectedClientDataElementUIController>();
            this.OnSelectedPlayer(this, new ClientConnectionDataEventArgs(controller.ConnectionId));
        }

        private void UpdateContainerSize()
        {
            var sizeY = YOffset + ((this.clientElementPrefabHeight + DistanceBetweenElements) * this.connectionIdClientElement.Count);
            this.contentRectTransform.sizeDelta = new Vector2(this.contentRectTransform.sizeDelta.x, sizeY);
        }
    }
}
