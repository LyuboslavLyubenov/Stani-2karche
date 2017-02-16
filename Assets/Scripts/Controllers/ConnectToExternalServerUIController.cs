namespace Assets.Scripts.Controllers
{
    using System.Reflection;

    using Assets.Scripts.Utils;

    using UnityEngine;
    using UnityEngine.UI;

    using DTOs;

    using EventArgs;
    using Network;
    using Notifications;

    using Zenject;

    public class ConnectToExternalServerUIController : MonoBehaviour
    {
        private const int ConnectionTimeoutInSeconds = 5;
        
        public GameObject LoadingUI;

        public Text IPText;

        [Inject]
        private EveryBodyVsTheTeacher.ServerSelectPlayerTypeUIController everyBodyVsTheTeacherSelectPlayerTypeUiController;

        [Inject]
        private CreatedGameInfoReceiverService gameInfoReceiverService;

        [Inject]
        private BasicExamServerSelectPlayerTypeUIController selectPlayerTypeUIController;

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
            var methodName = "OnConnectingTo " + gameType;
            var method = this.GetType()
                .GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);

            if (method == null)
            {
                NotificationsServiceController.Instance.AddNotification(Color.red, "Неподържан вид игра", 10);
                return;
            }

            var dto = this.ParseJSONToSpecificGameTypeDTO(gameType, gameTypeJSON);

            method.Invoke(this, new [] { dto });
        }

        private object ParseJSONToSpecificGameTypeDTO(string gameType, string gameTypeJSON)
        {
            var type = ServerGameTypeUtils.GetGameServerType(gameType);
            var dto = JsonUtility.FromJson(gameTypeJSON, type);
            return dto;
        }
        
        private void OnConnectingToBasicExam(object gameInfo_DTO)
        {
            var gameInfo = (BasicExamGameInfo_DTO)gameInfo_DTO;
            this.selectPlayerTypeUIController.gameObject.SetActive(true);
            this.selectPlayerTypeUIController.Initialize(gameInfo);
        }

        private void OnConnectingToEveryBodyVsTheTeacher(object gameInfo_DTO)
        {
            var gameInfo = (EverybodyVsTheTeacherGameInfo_DTO)gameInfo_DTO;
            this.everyBodyVsTheTeacherSelectPlayerTypeUiController.gameObject.SetActive(true);
            this.everyBodyVsTheTeacherSelectPlayerTypeUiController.Initialize(gameInfo);
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
