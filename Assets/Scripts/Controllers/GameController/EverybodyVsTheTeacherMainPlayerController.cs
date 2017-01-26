namespace Assets.Scripts.Controllers.GameController
{

    using Assets.Scripts.Network.NetworkManagers;
    using Assets.Scripts.Notifications;
    using Assets.Scripts.Utils.Unity;

    using UnityEngine;

    using EventArgs = System.EventArgs;

    public class EverybodyVsTheTeacherMainPlayerController : ExtendedMonoBehaviour
    {
        public GameObject LoadingUI;
        public GameObject UnableToConnectUI;

        public ClientNetworkManager NetworkManager;
        public NotificationsServiceController NotificationController;
        public AvailableJokersUIController JokersUI;
        public SecondsRemainingUIController SecondsRemainingUIController;
        public MarkPanelController MarkUIController;
        public SurrenderConfirmUIController SurrenderConfirmUIController;
        public ClientChooseCategoryUIController ChooseCategoryUIController;
        public UnableToConnectUIController UnableToConnectUIController;
        public QuestionUIController QuestionUIController;

        private void Start()
        {
            this.LoadControllers();
            this.AttachEventHandlers();
            this.ConnectToServer();

            this.LoadingUI.SetActive(true);
            this.ConnectToServer();
        }

        private void OnConnectedToServer(object sender, EventArgs args)
        {
            this.LoadingUI.SetActive(false);
            this.UnableToConnectUI.SetActive(false);
        }

        private void OnDisconnectedFromServer(object sender, EventArgs args)
        {
            this.UnableToConnectUI.SetActive(true);
        }

        private void OnFoundServerIP(string ip)
        {
            this.UnableToConnectUIController.ServerIP = ip;

            try
            {
                this.NetworkManager.ConnectToHost(ip);
            }
            catch
            {
                this.OnFoundServerIPError();
            }
        }

        private void OnFoundServerIPError()
        {
            this.CoroutineUtils.WaitForSeconds(1f, this.ConnectToServer);
        }

        private void LoadControllers()
        {  
        
        }

        private void AttachEventHandlers()
        {
            this.NetworkManager.OnConnectedEvent += this.OnConnectedToServer;
            this.NetworkManager.OnDisconnectedEvent += this.OnDisconnectedFromServer;
        }

        private void ConnectToServer()
        {
            NetworkManagerUtils.Instance.GetServerIp(this.OnFoundServerIP, this.OnFoundServerIPError);
        }
    }

}