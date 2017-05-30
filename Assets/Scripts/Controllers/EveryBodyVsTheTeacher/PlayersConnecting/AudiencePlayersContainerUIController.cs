namespace Controllers.EveryBodyVsTheTeacher.PlayersConnecting
{

    using System.Collections.Generic;
    using System.Linq;

    using EventArgs;

    using Interfaces.Controllers;
    using Interfaces.Network.NetworkManager;

    using UnityEngine;
    using UnityEngine.UI;

    using Utils.Unity;

    using Zenject.Source.Usage;

    public class AudiencePlayersContainerUIController : MonoBehaviour, IAudiencePlayersContainerUIController
    {
        private const int AudiencePlayerObjectStartPositionX = -550;

        private readonly Vector2 StartPosition = new Vector2(AudiencePlayerObjectStartPositionX, 0);

        public GameObject DotsInAudienceContainer;
        
        public ObjectsPool AudienceObjectsPool;
        
        private readonly Queue<GameObject> audiencePlayerObjects = new Queue<GameObject>();
        private readonly Dictionary<int, GameObject> connectionIdAudienceObj = new Dictionary<int, GameObject>();

        void Awake()
        {
            Physics2D.gravity = new Vector3(6f, 0f, 0f);
        }

        void Start()
        {
            this.DotsInAudienceContainer.SetActive(false);
        }
        
        private void HideDotsIfNoMoreThanFourPlayersAreConnected()
        {
            if (this.connectionIdAudienceObj.Count <= 3)
            {
                this.DotsInAudienceContainer.SetActive(false);
            }
        }
        
        private GameObject GetAudienceObjectFromPoolWithStartPosition()
        {
            var audiencePlayerTransform = this.AudienceObjectsPool.Get();

            var rectTransform = audiencePlayerTransform.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = this.StartPosition;

            return audiencePlayerTransform.gameObject;
        }

        public void ShowAudiencePlayer(int connectionId, string username)
        {
            if (this.audiencePlayerObjects.Count >= 3)
            {
                var audienceObjectToBeDeactivated = this.audiencePlayerObjects.Dequeue();
                audienceObjectToBeDeactivated.SetActive(false);

                this.DotsInAudienceContainer.SetActive(true);
            }

            var newAudienceObject = this.GetAudienceObjectFromPoolWithStartPosition();

            var textComponent = newAudienceObject.GetComponentInChildren<Text>();
            textComponent.text = username;

            this.connectionIdAudienceObj.Add(connectionId, newAudienceObject);
            this.audiencePlayerObjects.Enqueue(newAudienceObject);
        }

        public void HideAudiencePlayer(int connectionId)
        {
            if (!this.IsOnScreen(connectionId))
            {
                return;
            }

            var obj = this.connectionIdAudienceObj[connectionId];
            obj.SetActive(false);

            this.connectionIdAudienceObj.Remove(connectionId);
            this.HideDotsIfNoMoreThanFourPlayersAreConnected();
        }

        public void HideAll()
        {
            var audienceConnectionIds = this.connectionIdAudienceObj.Select(c => c.Key).ToArray();

            for (int i = 0; i < audienceConnectionIds.Length; i++)
            {
                var connectionId = audienceConnectionIds[i];
                this.HideAudiencePlayer(connectionId);
            }
        }

        public bool IsOnScreen(int connectionId)
        {
            return this.connectionIdAudienceObj.ContainsKey(connectionId);
        }
    }
}