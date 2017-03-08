namespace Assets.Scripts.Controllers.GameController
{

    using System;

    using Assets.Scripts.Commands;
    using Assets.Scripts.Commands.Server;
    using Assets.Scripts.Controllers.EveryBodyVsTheTeacher;
    using Assets.Scripts.Extensions;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Scripts.Localization;
    using Assets.Scripts.Notifications;
    using Assets.Scripts.StateMachine;
    using Assets.Scripts.Utils;
    using Assets.Scripts.Utils.Unity;
    using Assets.Zenject.Source.Usage;

    using UnityEngine;
    using UnityEngine.UI;

    public class EverybodyVsTheTeacherMainPlayerController : ExtendedMonoBehaviour
    {
        private StateMachine StateMachine;

        [Inject]
        private NotConnectedToServerState notConnectedToServerState;

        void Start()
        {
            this.StateMachine = new StateMachine(this.notConnectedToServerState);
        }
    }

    public class NotConnectedToServerState : IState
    {
        [Inject]
        private GameObject LoadingUI;

        [Inject]
        private GameObject UnableToConnectUI;

        [Inject]
        private UnableToConnectUIController unableToConnectUIController;

        [Inject]
        private IClientNetworkManager clientNetworkManager;

        [Inject ]
        private ConnectedToServerButNotStartedGameState nextState;

        private Timer_ExecuteMethodAfterTime notFoundServerIPTimer;

        private void OnConnectedToServer(object sender, EventArgs args)
        {
            this.LoadingUI.SetActive(false);
            this.UnableToConnectUI.SetActive(false);

            var commandData = NetworkCommandData.From<MainPlayerConnectingCommand>();
            this.clientNetworkManager.SendServerCommand(commandData);

            var connectedMsg = LanguagesManager.Instance.GetValue("EveryBodyVsTheTeacher/ConnectedToServer");
            NotificationsesController.Instance.AddNotification(Color.blue, connectedMsg);
        }

        private void OnDisconnectedFromServer(object sender, EventArgs args)
        {
            this.UnableToConnectUI.SetActive(true);
        }

        private void OnFoundServerIP(string ip)
        {
            this.unableToConnectUIController.ServerIP = ip;
            this.clientNetworkManager.ConnectToHost(ip);
        }

        private void OnFoundServerIPError()
        {
            this.notFoundServerIPTimer.Reset();
        }

        private void GetServerIpAndConnectToServer()
        {
            NetworkManagerUtils.Instance.GetServerIp(this.OnFoundServerIP, this.OnFoundServerIPError);
        }

        private void AttachEventHandlers()
        {
            this.clientNetworkManager.OnConnectedEvent += this.OnConnectedToServer;
            this.clientNetworkManager.OnDisconnectedEvent += this.OnDisconnectedFromServer;
        }

        private void ConfigureNotFoundServerIPTimer()
        {
            this.notFoundServerIPTimer = TimerUtils.ExecuteAfter(1f, this.GetServerIpAndConnectToServer);
            this.notFoundServerIPTimer.AutoDispose = false;
            this.notFoundServerIPTimer.RunOnUnityThread = true;
        }

        public void OnStateEnter(StateMachine stateMachine)
        {
            this.LoadingUI.SetActive(true);
            this.UnableToConnectUI.SetActive(false);

            this.ConfigureNotFoundServerIPTimer();
            this.AttachEventHandlers();
        }

        public void OnStateExit(StateMachine stateMachine)
        {
            this.notFoundServerIPTimer.Stop();
            this.notFoundServerIPTimer.Dispose();
            this.notFoundServerIPTimer = null;
        }
    }

    public class ConnectedToServerButNotStartedGameState : IState
    {
        [Inject]
        private IClientNetworkManager networkManager;

        [Inject]
        private StartGameButtonUIController startGameButtonUIController;

        private void ActivateStartGameButton()
        {
            this.startGameButtonUIController.gameObject.SetActive(true);
        }

        public void OnStateEnter(StateMachine stateMachine)
        {
            throw new NotImplementedException();
        }

        public void OnStateExit(StateMachine stateMachine)
        {
            throw new System.NotImplementedException();
        }
    }

}