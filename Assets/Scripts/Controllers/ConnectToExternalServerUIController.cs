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

    public class ConnectToExternalServerUIController : MonoBehaviour
    {
        private const int ConnectionTimeoutInSeconds = 5;

        public CreatedGameInfoReceiverService GameInfoReceiverService;

        public BasicExamServerSelectPlayerTypeUIController SelectPlayerTypeUIController;

        public EveryBodyVsTheTeacher.ServerSelectInfoUIController EveryBodyVsTheTeacherSelectInfoUIController;

        public GameObject LoadingUI;
        public Text IPText;

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

            this.GameInfoReceiverService.StopReceivingFrom(this.ip);
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
            var method = this.GetType()
                .GetMethod("OnConnectingTo " + gameType, BindingFlags.Instance | BindingFlags.NonPublic);

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
            this.SelectPlayerTypeUIController.gameObject.SetActive(true);
            this.SelectPlayerTypeUIController.Initialize(gameInfo);
        }

        private void OnConnectingToEveryBodyVsTheTeacher(object gameInfo_DTO)
        {
            var gameInfo = (EverybodyVsTheTeacherGameInfo_DTO)gameInfo_DTO;
            this.EveryBodyVsTheTeacherSelectInfoUIController.gameObject.SetActive(true);
            this.EveryBodyVsTheTeacherSelectInfoUIController.Initialize(gameInfo);
        }

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
    }
}
