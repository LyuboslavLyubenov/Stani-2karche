namespace Assets.Scripts.Controllers
{

    using System.Linq;

    using UnityEngine;
    using UnityEngine.UI;

    using DTOs;
    using Enums;
    using EventArgs;
    using Network;
    using Notifications;

    public class ConnectToExternalServerUIController : MonoBehaviour
    {
        private const int ConnectionTimeoutInSeconds = 5;

        public CreatedGameInfoReceiverService GameInfoReceiverService;
        public NotificationsServiceController NotificationService;
        public BasicExamServerSelectPlayerTypeUIController SelectPlayerTypeUIController;
        public GameObject LoadingUI;
        public Text IPText;

        private float elapsedTimeTryingToConnect = 0;

        private string ip;

        private bool connecting = false;

        public void TryToConnect(string ip)
        {
            this.elapsedTimeTryingToConnect = 0;
            this.ip = ip;
            this.connecting = true;

            this.GameInfoReceiverService.ReceiveFrom(ip, this.OnReceivedGameInfo);
            this.LoadingUI.SetActive(true);
        }

        public void TryToConnect()
        {
            this.TryToConnect(this.IPText.text);
        }

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
                this.LoadingUI.SetActive(false);
                this.NotificationService.AddNotification(Color.red, "Няма връзка със сървъра");

                try
                {
                    this.GameInfoReceiverService.StopReceivingFrom(this.ip);
                }
                catch
                {
                
                }

                this.connecting = false;
            }
        }

        private void OnReceivedGameInfo(GameInfoReceivedDataEventArgs args)
        {
            this.LoadingUI.SetActive(false);
            this.connecting = false;

            var gameType = args.GameInfo.GameTypeFullName.Split('.')
                .Last();

            switch (gameType)
            {
                case "BasicExam":
                    var basicExamGameInfo = JsonUtility.FromJson<BasicExamGameInfo_DTO>(args.JSON);
                    this.OnConnectingToBasicExam(basicExamGameInfo);
                    break;

                default:
                    this.NotificationService.AddNotification(Color.red, "Неподържан вид игра", 10);
                    break;
            }
        }

        private void OnConnectingToBasicExam(BasicExamGameInfo_DTO gameInfo_DTO)
        {
            this.SelectPlayerTypeUIController.gameObject.SetActive(true);
            this.SelectPlayerTypeUIController.Initialize(gameInfo_DTO);
        }
    }

}
