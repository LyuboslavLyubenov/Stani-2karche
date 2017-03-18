using PlayersConnectingToTheServerState = StateMachine.EveryBodyVsTheTeacher.States.Server.PlayersConnectingToTheServerState;

namespace Assets.Scripts.Controllers.EveryBodyVsTheTeacher.PlayersConnecting
{

    using System.Collections.Generic;

    using Assets.Scripts.Controllers.EveryBodyVsTheTeacher.States.Server;
    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Scripts.Utils.Unity;
    using Assets.Zenject.Source.Usage;

    using UnityEngine;
    using UnityEngine.UI;

    public class AudiencePlayersContainerUIController : MonoBehaviour
    {
        private const int AudiencePlayerObjectStartPositionX = -550;

        private readonly Vector2 StartPosition = new Vector2(AudiencePlayerObjectStartPositionX, 0);

        public GameObject DotsInAudienceContainer;
        
        public ObjectsPool AudienceObjectsPool;

        [Inject]
        private PlayersConnectingToTheServerState state;

        [Inject]
        private IServerNetworkManager serverNetworkManager;

        private readonly Queue<GameObject> audiencePlayerObjects = new Queue<GameObject>();
        private readonly Dictionary<int, GameObject> connectionIdAudienceObj = new Dictionary<int, GameObject>();

        void Awake()
        {
            Physics2D.gravity = new Vector3(6f, 0f, 0f);
        }

        void Start()
        {
            this.state.OnAudiencePlayerConnected += this.OnAudiencePlayerConnected;
            this.state.OnAudiencePlayerDisconnected += this.OnAudiencePlayerDisconnected;

            this.DotsInAudienceContainer.SetActive(false);
        }

        private void OnAudiencePlayerConnected(object sender, ClientConnectionDataEventArgs args)
        {
            this.ShowAudiencePlayerOnScreen(args.ConnectionId);
        }

        private void OnAudiencePlayerDisconnected(object sender, ClientConnectionDataEventArgs args)
        {
            if (!this.IsOnScreen(args.ConnectionId))
            {
                return;
            }

            this.HideAudiencePlayerFromScreen(args.ConnectionId);
        }

        private void HideAudiencePlayerFromScreen(int connectionId)
        {
            var obj = this.connectionIdAudienceObj[connectionId];
            obj.SetActive(false);

            this.connectionIdAudienceObj.Remove(connectionId);
            this.HideDotsIfNoMoreThanFourPlayersAreConnected();
        }

        private void HideDotsIfNoMoreThanFourPlayersAreConnected()
        {
            if (this.connectionIdAudienceObj.Count <= 3)
            {
                this.DotsInAudienceContainer.SetActive(false);
            }
        }

        private void ShowAudiencePlayerOnScreen(int connectionId)
        {
            if (this.audiencePlayerObjects.Count >= 3)
            {
                var audienceObjectToBeDeactived = this.audiencePlayerObjects.Dequeue();
                audienceObjectToBeDeactived.SetActive(false);
                
                this.DotsInAudienceContainer.SetActive(true);
            }

            var newAudienceObject = this.GetAudienceObjectFromPoolWithStartPosition();

            this.GetUsernameFromServerAndSetItToAudienceObject(connectionId, newAudienceObject);
            
            this.connectionIdAudienceObj.Add(connectionId, newAudienceObject);
            this.audiencePlayerObjects.Enqueue(newAudienceObject);
        }

        private GameObject GetAudienceObjectFromPoolWithStartPosition()
        {
            var audiencePlayerTransform = this.AudienceObjectsPool.Get();

            var rectTransform = audiencePlayerTransform.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = this.StartPosition;

            return audiencePlayerTransform.gameObject;
        }

        private void GetUsernameFromServerAndSetItToAudienceObject(int connectionId, GameObject audienceObj)
        {
            var playerUsername = this.serverNetworkManager.GetClientUsername(connectionId);
            var textComponent = audienceObj.GetComponentInChildren<Text>();
            textComponent.text = playerUsername;
        }

        public bool IsOnScreen(int connectionId)
        {
            return this.connectionIdAudienceObj.ContainsKey(connectionId);
        }
    }

}