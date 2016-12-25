using UnityEngine;

namespace Assets.Scripts.Controllers
{

    using Assets.Scripts.Network;
    using Assets.Scripts.Notifications;
    using Assets.Scripts.Utils;

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

        void Start()
        {
            this.LoadControllers();
            this.AttachEventHandlers();
            this.ConnectToServer();

            this.LoadingUI.SetActive(true);
            this.ConnectToServer();
        }

        void OnConnectedToServer(object sender, EventArgs args)
        {
            this.LoadingUI.SetActive(false);
            this.UnableToConnectUI.SetActive(false);
        }

        void OnDisconnectedFromServer(object sender, EventArgs args)
        {
            this.UnableToConnectUI.SetActive(true);
        }

        void OnFoundServerIP(string ip)
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

        void OnFoundServerIPError()
        {
            this.CoroutineUtils.WaitForSeconds(1f, this.ConnectToServer);
        }

        void LoadControllers()
        {  
        
        }

        void AttachEventHandlers()
        {
            this.NetworkManager.OnConnectedEvent += this.OnConnectedToServer;
            this.NetworkManager.OnDisconnectedEvent += this.OnDisconnectedFromServer;
        }

        void ConnectToServer()
        {
            NetworkManagerUtils.Instance.GetServerIp(this.OnFoundServerIP, this.OnFoundServerIPError);
        }
    }

}