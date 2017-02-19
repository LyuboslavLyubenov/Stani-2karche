namespace Assets.Scripts.Controllers.Lobby
{

    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Network;
    using Assets.Scripts.Notifications;

    using UnityEngine;
    using UnityEngine.UI;

    using Zenject;

    public class ConnectToExternalServerUIController : MonoBehaviour
    {
        private const int ConnectionTimeoutInSeconds = 5;
        
        public GameObject LoadingUI;

        public Text IPText;

        [Inject]
        private CreatedGameInfoReceiverService gameInfoReceiverService;

        [Inject]
        private SelectPlayerTypeRouter SelectPlayerTypeRouter;

        private float elapsedTimeTryingToConnect = 0;
        private string ip;
        private bool connecting = false;

        // ReSharper disable once ArrangeTypeMemberModifiers
        void Update()
        {
            if (!this.connecting)
            {
                return;
            }

            this.elapsedTimeTryingToConnect += Time.deltaTime;

            if (this.elapsedTimeTryingToConnect >= ConnectionTimeoutInSeconds)
            {
                this.ConnectToServerTimeout();
            }
        }

        private void ConnectToServerTimeout()
        {
            this.connecting = false;
            this.LoadingUI.SetActive(false);

            NotificationsServiceController.Instance.AddNotification(Color.red, "Няма връзка със сървъра");

            this.gameInfoReceiverService.StopReceivingFrom(this.ip);
        }

        private void OnReceivedGameInfo(GameInfoReceivedDataEventArgs args)
        {
            this.LoadingUI.SetActive(false);
            this.connecting = false;

            var gameType = args.GameInfo.GameType;

            this.RouteReceivedGameInfo(gameType, args.JSON);
        }

        private void RouteReceivedGameInfo(string gameType, string gameTypeJSON)
        {
            this.SelectPlayerTypeRouter.Handle(gameType, gameTypeJSON);
        }
        
        public void TryToConnect(string ip)
        {
            this.elapsedTimeTryingToConnect = 0;
            this.ip = ip;
            this.connecting = true;

            this.gameInfoReceiverService.ReceiveFrom(ip, this.OnReceivedGameInfo);
            this.LoadingUI.SetActive(true);
        }

        public void TryToConnect()
        {
            this.TryToConnect(this.IPText.text);
        }
    }
}
