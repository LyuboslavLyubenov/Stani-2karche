namespace Controllers.EveryBodyVsTheTeacher.PlayersConnecting
{
    using System.Collections.Generic;
    using System.Linq;

    using Interfaces.Controllers;

    using UnityEngine;
    using UnityEngine.UI;

    using Utils.Unity;

    public class AudiencePlayersContainerUIController : MonoBehaviour, IAudiencePlayersContainerUIController
    {
        private class AudiencePlayerOnScreen : AudiencePlayer
        {
            public GameObject Obj { get; set; }
        }

        private class AudiencePlayer
        {
            public int ConnectionId { get; set; }

            public int OrderNumber { get; set; }

            public string Username { get; set; }
        }

        private const int AudiencePlayerObjectStartPositionX = -550;

        private readonly Vector2 StartPosition = new Vector2(AudiencePlayerObjectStartPositionX, 0);

        public GameObject DotsInAudienceContainer;
        
        public ObjectsPool AudienceObjectsPool;

        private readonly HashSet<AudiencePlayerOnScreen> objectsOnScreen = new HashSet<AudiencePlayerOnScreen>();
        private readonly HashSet<AudiencePlayer> playersShown = new HashSet<AudiencePlayer>();
    
        private int orderIndex = 0;

        void Awake()
        {
            Physics2D.gravity = new Vector3(6f, 0f, 0f);
        }

        void Start()
        {
            this.DotsInAudienceContainer.SetActive(false);
        }
        
        private GameObject GetAudienceObjectFromPoolWithStartPosition()
        {
            var audiencePlayerTransform = this.AudienceObjectsPool.Get();

            var rectTransform = audiencePlayerTransform.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = this.StartPosition;

            return audiencePlayerTransform.gameObject;
        }

        private void DeactivateFirstObject()
        {
            var objectToDeactivate = this.objectsOnScreen.OrderBy(o => o.OrderNumber)
                .First();
            objectToDeactivate.Obj.SetActive(false);
            this.objectsOnScreen.Remove(objectToDeactivate);
        }

        private void ShowLastConnectedOnScreen()
        {
            var player = this.playersShown.OrderByDescending(ps => ps.OrderNumber)
                .First(ps => this.objectsOnScreen.All(apos => apos.ConnectionId != ps.ConnectionId));
            this.ShowOnScreen(player);
        }

        private void ShowOnScreen(AudiencePlayer player)
        {
            var newAudienceObject = this.GetAudienceObjectFromPoolWithStartPosition();

            var textComponent = newAudienceObject.GetComponentInChildren<Text>();
            textComponent.text = player.Username;

            var playerOnScreen = new AudiencePlayerOnScreen()
                                       {
                                           ConnectionId = player.ConnectionId,
                                           Obj = newAudienceObject,
                                           OrderNumber = player.OrderNumber,
                                           Username = player.Username
                                       };

            this.objectsOnScreen.Add(playerOnScreen);
        }

        private void HideFromScreen(int connectionId)
        {
            var playerToHide = this.objectsOnScreen.FirstOrDefault(apos => apos.ConnectionId == connectionId);

            if (playerToHide == null)
            {
                return;
            }

            playerToHide.Obj.SetActive(false);
            this.objectsOnScreen.Remove(playerToHide);
        }
        
        public void ShowAudiencePlayer(int connectionId, string username)
        {
            if (this.playersShown.Count >= 3)
            {
                this.DeactivateFirstObject();
                this.DotsInAudienceContainer.SetActive(true);
            }

            var audiencePlayer = new AudiencePlayer()
                                 {
                                     ConnectionId = connectionId,
                                     OrderNumber = this.orderIndex++,
                                     Username = username
                                 };

            this.ShowOnScreen(audiencePlayer);
            this.playersShown.Add(audiencePlayer);
        }

        public void HideAudiencePlayer(int connectionId)
        {
            if (!this.IsOnScreen(connectionId))
            {
                return;
            }
            
            this.HideFromScreen(connectionId);

            var player = this.playersShown.First(ps => ps.ConnectionId == connectionId);
            this.playersShown.Remove(player);
            
            if (this.playersShown.Count <= 3 || this.objectsOnScreen.Count >= 3)
            {
                return;
            }

            this.ShowLastConnectedOnScreen();
        }

        public void HideAll()
        {
            var players = this.playersShown.ToArray();

            for (int i = 0; i < players.Length; i++)
            {
                var connectionId = players[i]
                    .ConnectionId;
                this.HideAudiencePlayer(connectionId);
            }
        }

        public bool IsOnScreen(int connectionId)
        {
            return this.playersShown.FirstOrDefault(ps => ps.ConnectionId == connectionId) != null;
        }
    }
}