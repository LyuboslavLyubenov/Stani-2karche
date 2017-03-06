namespace Assets.Scripts.Controllers.GameController
{

    using Assets.Scripts.Localization;
    using Assets.Scripts.Notifications;

    using Commands;
    using Commands.Server;

    using Network.NetworkManagers;
    using Utils.Unity;

    using UnityEngine;

    using EventArgs = System.EventArgs;

    public class EverybodyVsTheTeacherMainPlayerController : ExtendedMonoBehaviour
    {
        public GameObject LoadingUI;
        public GameObject UnableToConnectUI;
        
        public AvailableJokersUIController JokersUI;
        public SecondsRemainingUIController SecondsRemainingUIController;
        public MarkPanelController MarkUIController;
        public SurrenderConfirmUIController SurrenderConfirmUIController;
        public ClientChooseCategoryUIController ChooseCategoryUIController;
        public UnableToConnectUIController UnableToConnectUIController;
        public QuestionUIController QuestionUIController;

        void Start()
        {
            this.AttachEventHandlers();
            this.GetServerIpAndConnectToServer();

            this.LoadingUI.SetActive(true);
        }

        private void OnConnectedToServer(object sender, EventArgs args)
        {
            this.LoadingUI.SetActive(false);
            this.UnableToConnectUI.SetActive(false);

            var commandData = NetworkCommandData.From<MainPlayerConnectingCommand>();
            ClientNetworkManager.Instance.SendServerCommand(commandData);

            var connectedMsg = LanguagesManager.Instance.GetValue("EveryBodyVsTheTeacher/ConnectedToServer");
            NotificationsesController.Instance.AddNotification(Color.blue, connectedMsg);
        }

        private void OnDisconnectedFromServer(object sender, EventArgs args)
        {
            this.UnableToConnectUI.SetActive(true);
        }

        private void OnFoundServerIP(string ip)
        {
            this.UnableToConnectUIController.ServerIP = ip;
            ClientNetworkManager.Instance.ConnectToHost(ip);
            this.OnFoundServerIPError();
        }

        private void OnFoundServerIPError()
        {
            this.CoroutineUtils.WaitForSeconds(1f, this.GetServerIpAndConnectToServer);
        }
        
        private void AttachEventHandlers()
        {
            ClientNetworkManager.Instance.OnConnectedEvent += this.OnConnectedToServer;
            ClientNetworkManager.Instance.OnDisconnectedEvent += this.OnDisconnectedFromServer;
        }

        private void GetServerIpAndConnectToServer()
        {
            NetworkManagerUtils.Instance.GetServerIp(this.OnFoundServerIP, this.OnFoundServerIPError);
        }
    }

}