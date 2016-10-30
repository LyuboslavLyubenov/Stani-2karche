using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ConnectedClientsUIController : ExtendedMonoBehaviour
{
    const int YOffset = 75;
    const int DistanceBetweenElements = 5;

    public EventHandler<ClientConnectionDataEventArgs> OnSelectedPlayer = delegate
    {
    };

    public Transform Content;
    public ServerNetworkManager NetworkManager;
    public ObjectsPool ConnectedClientsDataPool;

    float clientElementPrefabHeight;

    RectTransform contentRectTransform;

    Dictionary<int, Transform> connectionIdClientElement = new Dictionary<int, Transform>();

    void Start()
    {
        var connectedClientDataPrefab = Resources.Load<GameObject>("Prefabs/ConnectedClientElementData");
        var rectTransform = connectedClientDataPrefab.GetComponent<RectTransform>();

        clientElementPrefabHeight = rectTransform.sizeDelta.y;
        ConnectedClientsDataPool.Prefab = connectedClientDataPrefab.transform;

        contentRectTransform = Content.GetComponent<RectTransform>();

        NetworkManager.OnClientConnected += OnClientConnected;
        NetworkManager.OnClientDisconnected += OnClientDisconnected;
        NetworkManager.OnClientSetUsername += OnClientSetUsername;
    }

    void OnClientConnected(object sender, ClientConnectionDataEventArgs args)
    {
        var connectionId = args.ConnectionId;

        if (!connectionIdClientElement.ContainsKey(connectionId))
        {
            AddConnectedClientToList(connectionId);
            UpdateContainerSize();
        }
    }

    void OnClientDisconnected(object sender, ClientConnectionDataEventArgs args)
    {
        if (connectionIdClientElement.ContainsKey(args.ConnectionId))
        {
            var connectionId = args.ConnectionId;
            var clientElement = connectionIdClientElement[connectionId];
            var button = clientElement.GetComponent<Button>();

            button.onClick.RemoveAllListeners();
            clientElement.gameObject.SetActive(false);

            connectionIdClientElement.Remove(args.ConnectionId);

            UpdateContainerSize();
        }
    }

    void OnClientSetUsername(object sender, ConnectedClientDataEventArgs args)
    {
        var connectionId = args.ClientData.ConnectionId;

        if (!connectionIdClientElement.ContainsKey(connectionId))
        {
            return;
        }
            
        var controller = connectionIdClientElement[connectionId].GetComponent<ConnectedClientDataElementUIController>();
        controller.Fill(args.ClientData);
    }

    void AddConnectedClientToList(int connectionId)
    {
        var connectedClientElement = ConnectedClientsDataPool.Get();
        var rectTransform = connectedClientElement.GetComponent<RectTransform>();
        var newY = YOffset + ((clientElementPrefabHeight + DistanceBetweenElements) * connectionIdClientElement.Count);
        rectTransform.anchoredPosition = new Vector2(0, -newY);

        var username = NetworkManager.GetClientUsername(connectionId);
        var clientData = new ConnectedClientData(connectionId, username);
        var connectedClientElementController = connectedClientElement.GetComponent<ConnectedClientDataElementUIController>();
        CoroutineUtils.WaitForFrames(0, () => connectedClientElementController.Fill(clientData));
        
        var button = connectedClientElement.transform.GetComponent<Button>();
        button.onClick.AddListener(ClickedOnPlayer);

        connectionIdClientElement.Add(connectionId, connectedClientElement);
    }

    void ClickedOnPlayer()
    {
        var obj = EventSystem.current.currentSelectedGameObject;
        var controller = obj.GetComponent<ConnectedClientDataElementUIController>();
        OnSelectedPlayer(this, new ClientConnectionDataEventArgs(controller.ConnectionId));
    }

    void UpdateContainerSize()
    {
        var sizeY = YOffset + ((clientElementPrefabHeight + DistanceBetweenElements) * connectionIdClientElement.Count);
        contentRectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x, sizeY);
    }

}
